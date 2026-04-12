FROM steamcmd/steamcmd:cachyos-3 AS base

# ENV ASPNETCORE_ENVIRONMENT="Production"
# ENV CS2_INSTALL_CSS="true"
# ENV CS2L_SERVER__PROGRAM="${STEAMAPPDIR}/game/bin/linuxsteamrt64/cs2"
# ENV CS2L_SERVER__SYSTEMUSER="${USER}"
# ENV CS2L_SERVER__WORKINGDIRECTORY="${STEAMAPPDIR}/game/bin/linuxsteamrt64/"
# ENV DOTNET_ENVIRONMENT="Production"

ENV ASPNETCORE_URLS="https://+;http://+"
ENV DOTNET_EnableDiagnostics=1
ENV DOTNET_EnableDiagnostics_Debugger=1
ENV DOTNET_EnableDiagnostics_IPC=1
ENV DOTNET_gcServer=1
ENV DOTNET_GCRetainVM=0
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_RUNNING_IN_CONTAINERS=true
ENV DOTNET_SYSTEM_RUNTIME_CACHING_TRACING=true

RUN pacman --noconfirm --needed -Syu \
    ca-certificates \
    \
    # .NET deps
    gcc-libs \
    glibc \
    icu \
    libgcc \
    openssl \
    tzdata \
    \
    # CS2 [launcher] deps
    lib32-nvidia-utils \
    lib32-opencl-nvidia \
    patchelf \
    vulkan-icd-loader \
    \
    # Debug deps
    gdb \
    procps-ng

RUN pacman --noconfirm -Scc && rm -rf /tmp/* && rm -rf /var/tmp/*

RUN mkdir -p "${HOME}" && chown -R "${USER}:${USER}" "${HOME}"

USER ${USER}
WORKDIR ${HOME}

EXPOSE 80/tcp \
    443/tcp \
    27015-27030/tcp \
    27035-27037/tcp \
    27000-27031/udp \
    27036/udp