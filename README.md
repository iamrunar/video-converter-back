# Hi

## Dockerization

Also see built-in ability
https://learn.microsoft.com/en-us/dotnet/core/docker/publish-as-container

## DI

### for Github

1. prepaire
    - di host
        * `export CR_PAT=YOUR_TOKEN` YOUR_TOKEN from https://github.com/settings/tokens
        * `echo $CR_PAT | docker login ghcr.io -u USERNAME --password-stdin`
    - install ruby
        * `sudo apt update`
        * `sudo apt install ruby-full`
    - install sinatra (see https://github.com/sinatra/sinatra)
        * `gem install sinatra`
        * `gem install puma`

2. upload watcher-script
    - `scp watcher-script.rb root@5.63.155.204:/opt/di-server/`

3. execute DI Server
    - `ruby watcher-script.rb -o 0.0.0.0 -p 4567`
    - or in background: `nohup ruby watcher-script.rb -o 0.0.0.0 -p 4567 &> /dev/null &`
