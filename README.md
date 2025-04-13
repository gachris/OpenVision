# OpenVision

[![Build Status](https://img.shields.io/badge/build-passing-brightgreen)](https://github.com/gachris/OpenVision)  
[![License](https://img.shields.io/badge/license-MIT-blue)](LICENSE)

**OpenVision** is a powerful and modular Computer Vision SDK and full-stack web platform. It enables developers to build scalable, real-time, vision-powered applications for both desktop and web environments. With integrated support for authentication, containerization, and deployment, OpenVision accelerates your journey from prototype to production.

---

## 📚 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Running the Project](#running-the-project)
- [Client Library Installation](#client-library-installation)
- [Contributing](#contributing)
- [License](#license)
- [Acknowledgments](#acknowledgments)

---

## 🧬 Overview

OpenVision offers both a versatile Computer Vision SDK and a production-ready web template (`OpenVision.Web.Template`) to streamline development of computer vision solutions.

### 🔧 Web Template Components

- **`OpenVision.Server`** – Backend APIs and core logic  
- **`OpenVision.Client`** – ASP.NET Core frontend  
- **`OpenVision.IdentityServer`** – Authentication and authorization using [Skoruba Duende IdentityServer](https://github.com/skoruba/Duende.IdentityServer.Admin)  
- **Docker & Aspire Integration** – For containerized and cloud-native workflows

### 🔬 SDK Capabilities

- Feature detection: AKAZE, ORB, SIFT, BRISK  
- Feature matching & homography estimation  
- Augmented Reality (AR) overlays  
- WebSocket support for real-time interactions  
- Cross-platform support (MAUI, WPF, WinUI)

---

## 🚀 Features

### Web Template

- ✅ Modular, scalable architecture  
- 🔐 Integrated identity management (SSO, OAuth2, OIDC)  
- 🐳 Docker-ready for cloud and on-prem deployments  
- 🧹 Extensible for enterprise or startup use cases  

### SDK

- 🎯 Feature Detection: AKAZE, ORB, BRISK, SIFT  
- 🔗 Feature Matching: Brute Force, FLANN, etc.  
- 🧠 Homography & AR: Planar object detection, marker-based tracking  
- 🔍 Image Recognition: On-device or cloud-integrated  
- 🖥️ UI Frameworks: MAUI, WPF, WinUI  
- 🔄 Real-Time: Bi-directional vision task updates via WebSocket  

---

## 🧰 Prerequisites

Ensure the following tools are installed:

- [.NET SDK 8.0+](https://dotnet.microsoft.com/download)
- Visual Studio 2022+ (with ASP.NET + Docker workloads)
- Docker Desktop
- Node.js (for frontend builds)
- Git
- [mkcert](https://github.com/FiloSottile/mkcert) (for local HTTPS)

---

## 🏁 Getting Started

### 1. Install the Web Template

```bash
dotnet new install OpenVision.Web.Template
```

### 2. Create a New Project

```bash
dotnet new OpenVision.Web.Template -n MyOpenVisionApp
```

---

## ▶️ Running the Project

### 🔹 Option 1: Docker Compose (Recommended)

#### Update Hosts File

Add these entries to your system's hosts file:

```txt
127.0.0.1 openvision.com www.openvision.com api.openvision.com auth.openvision.com account.openvision.com account-api.openvision.com
```

- **Linux**: `/etc/hosts`  
- **Windows**: `C:\Windows\System32\drivers\etc\hosts`

#### Certificate Setup

##### Create the Root Certificate

Use [mkcert](https://github.com/FiloSottile/mkcert) to generate local self-signed certificates.

> **Note:** On Windows, `mkcert --install` must be executed under **elevated Administrator** privileges.

```powershell
cd shared/nginx/certs
mkcert --install
copy $env:LOCALAPPDATA\mkcert\rootCA-key.pem ./cacerts.pem
copy $env:LOCALAPPDATA\mkcert\rootCA.pem ./cacerts.crt
```

##### Create Certificates for openvision.com

Generate certificates for `openvision.com` including wildcards for subdomains. This ensures compatibility with the nginx proxy setup.

```powershell
cd shared/nginx/certs
mkcert -cert-file openvision.com.crt -key-file openvision.com.key openvision.com *.openvision.com
mkcert -pkcs12 openvision.com.pfx openvision.com *.openvision.com
```

#### Start Services

```bash
docker-compose build
docker-compose up -d
```

---

### 🔹 Option 2: Aspire AppHost (Cloud-native Dev)

```bash
cd src/OpenVision.Aspire.AppHost
dotnet run
```

---

### 🔹 Option 3: Manual Startup (Debug/Dev Mode)

Start these projects individually:

- `OpenVision.Server`  
- `OpenVision.Client`  
- `OpenVision.IdentityServer.Admin`  
- `OpenVision.IdentityServer.STS.Identity`  
- `OpenVision.IdentityServer.Admin.Api`

Use Visual Studio or `dotnet run` CLI.

---

## 📦 Client Library Installation

Install JavaScript dependencies for IdentityServer UIs:

```bash
cd src/OpenVision.IdentityServer.Admin
npm install

cd src/OpenVision.IdentityServer.STS.Identity
npm install
```

For advanced identity customization, see the [Skoruba IdentityServer Admin Guide](https://github.com/skoruba/Duende.IdentityServer.Admin).

---

## 🤝 Contributing

We love contributions! To get started:

```bash
# Clone your fork
git clone https://github.com/YOUR_USERNAME/OpenVision.git
cd OpenVision

# Create a feature branch
git checkout -b feature/my-feature

# Push changes
git push origin feature/my-feature
```

Then open a pull request 🚀

---

## 📄 License

OpenVision is licensed under the MIT License – see the [LICENSE](LICENSE) file for details.

---

## 🙌 Acknowledgments

- OpenVision Community & Contributors  
- Inspired by modern .NET, DevOps, and Computer Vision engineering best practices

