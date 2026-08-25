# https://hub.docker.com/r/microsoft/devcontainers-dotnet
# You can update the version of the base image with .NET and SDK only checking it together with the related VSCode extensions
# Please see the detailed comment in devcontainer.json
FROM mcr.microsoft.com/devcontainers/dotnet:1.5.5-9.0-noble

# It is necessary to solve the problem described in this issue https://github.com/yarnpkg/yarn/issues/9218
# Download and install a new key
RUN curl -sS https://dl.yarnpkg.com/debian/pubkey.gpg | gpg --dearmor | tee /etc/apt/keyrings/yarn-archive-keyring.gpg > /dev/null
# Set a new path to the key
RUN echo "deb [signed-by=/etc/apt/keyrings/yarn-archive-keyring.gpg] https://dl.yarnpkg.com/debian stable main" | tee /etc/apt/sources.list.d/yarn.list

# This repo still targets net8.0 while the base image above ships the .NET 9 SDK and the .NET 9 runtime only.
# The .NET 9 SDK builds net8.0 projects fine, but dotnet run/test/ef need the matching .NET 8 runtime to be present,
# otherwise they fail with "You must install or update .NET to run this application. Framework: 'Microsoft.AspNetCore.App', version '8.0.0'".
# When this repo is migrated to net9.0 (see inner-circle-time-api) this block and the dotnet-ef version pin below can be dropped.
RUN curl -fsSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh && \
    chmod +x /tmp/dotnet-install.sh && \
    /tmp/dotnet-install.sh --channel 8.0 --runtime aspnetcore --install-dir /usr/share/dotnet --no-path && \
    rm /tmp/dotnet-install.sh

# installing EntityFramework Core tooling for migrations
# the version is kept in sync with the Microsoft.EntityFrameworkCore.* packages of this repo which are 8.x
RUN dotnet tool install --global dotnet-ef --version 8.0.8 && \
    # installing tooling for tests coverage and its reporting
    dotnet tool install --global dotnet-coverage --version 18.1.0 && \
    dotnet tool install --global dotnet-reportgenerator-globaltool --version 5.4.18

# https://stackoverflow.com/a/72727052
# we need to manually add it to PATH otherwise it cannot find dotnet ef tool
ENV PATH="$PATH:/root/.dotnet/tools"

# installing jre, curl, and unzip for Karate 
# https://stackoverflow.com/a/44154721
RUN apt-get update && \
    DEBIAN_FRONTEND=noninteractive \
    apt-get -y install --no-install-recommends \
    curl unzip default-jre-headless

RUN curl -o /karate.jar -L 'https://github.com/intuit/karate/releases/download/v1.5.1/karate-1.5.1.jar'

# [Optional] Uncomment this line to install global node packages.
# RUN su vscode -c "source /usr/local/share/nvm/nvm.sh && npm install -g <your-package-here>" 2>&1