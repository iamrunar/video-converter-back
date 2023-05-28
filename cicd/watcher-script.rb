require 'sinatra'
require 'json'

get '/' do
    'Hi! I''m Github web-hook handler!'
end

post '/test-payload' do
  push = JSON.parse(request.body.read)
  puts "I got some JSON: #{push.inspect}"
end

post '/event_handler' do
    payload_body = request.body.read
    # should export SECRET_TOKEN=...
    # https://docs.github.com/en/webhooks-and-events/webhooks/securing-your-webhooks
    verify_signature(payload_body)

    @payload = JSON.parse(payload_body)

    logger.info "I got HTTP_X_GITHUB_EVENT : #{request.env['HTTP_X_GITHUB_EVENT']}"
  
    case request.env['HTTP_X_GITHUB_EVENT']
    when "package"

      if @payload["action"] == "published"
        process_published_event(@payload["package"])
      end

    end
  end
  
  helpers do
    def process_published_event(package)
        package_version=package['package_version']
        container_metadata=package_version['container_metadata'] 
        tag =container_metadata['tag']
        tag_name = tag['name']
        package_url = package_version['package_url']

        logger.info  "It's #{package['id']}. Package url #{package_url}. Tag name #{tag_name} #{tag_name.nil?} #{tag_name.empty?}"
        unless tag_name.empty?
            upgrateAndRestartContainer(package_url)
        end
    end

    def upgrateAndRestartContainer(package_url)
        logger.info  "1/4. Pull container"
        logger.info  `docker pull ghcr.io/iamrunar/video-converter-back:feature-deploy_setup`

        logger.info  "2/4. Stop container"
        logger.info  `docker stop video-converter-back`

        logger.info  "3/4. Remove container"
        logger.info  `docker rm video-converter-back`

        logger.info  "4/4. Remove container"
        logger.info  `docker run -d --name video-converter-back -p 5104:80 #{package_url}`
    end
  end

  def verify_signature(payload_body)
    signature = 'sha256=' + OpenSSL::HMAC.hexdigest(OpenSSL::Digest.new('sha256'), ENV['SECRET_TOKEN'], payload_body)
    return halt 500, "Signatures didn't match!" unless Rack::Utils.secure_compare(signature, request.env['HTTP_X_HUB_SIGNATURE_256'])
    logger.info "Signature OK"
  end