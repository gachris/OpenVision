# OpenVision

**OpenVision** is designed for computer vision applications. It provides various feature extraction, matching, recognition, and AR camera functionalities. The project is modular and allows users to integrate robust vision features into their applications, supporting platforms like .NET MAUI, WPF, and WinUI.

## Table of Contents

- [Project Overview](#project-overview)
- [Features](#features)
- [Dependencies](#dependencies)
- [Building and Running](#building-and-running)
- [Usage](#usage)
- [Using NuGet Packages](#using-nuget-packages)
- [Contributing](#contributing)
- [License](#license)

## Project Overview

OpenVision is a versatile project focused on feature detection, matching, and image recognition for computer vision tasks. It is built to provide core vision-related functionalities and supports both cloud and on-device recognition through a modular structure.

## Features

- **Feature Detection:** Support for various feature detectors such as AKAZE, ORB, BRISK, and SIFT.
- **Feature Matching:** Configurable feature matchers using different algorithms such as BFMatcher (Brute Force) and others.
- **Image Recognition:** Local and cloud-based image recognition systems.
- **Homography & AR:** Built-in support for homography calculations and integration with augmented reality (AR) camera setups for MAUI, WPF, and WinUI platforms.
- **Dataset Handling:** Efficient methods for handling and serializing datasets related to computer vision tasks.
- **WebSocket Communication:** Supports WebSocket communication to interact with external systems for results and recognition tasks.

### Key Directories

- **Configuration**: Holds various feature detector and matcher option configurations (e.g., AKAZE, BRISK, ORB, SIFT).
- **Dataset**: Manages dataset-related functionalities such as target handling and serialization.
- **DataTypes**: Core data models like `FeatureMatchingResult`, `ImageRequest`, and `HomographyResult`.
- **Features2d**: Contains feature extraction and matching utilities.
- **Reco**: Recognition-related functionalities, including cloud and local recognition implementations.
- **OpenVision.Maui**: Platform-specific implementation for .NET MAUI.
- **OpenVision.Wpf**: Platform-specific implementation for WPF applications.
- **OpenVision.WinUI**: Platform-specific implementation for WinUI applications.
- **OpenVision.Shared**: Shared utilities and responses used across platforms.

## Dependencies

- **.NET 8.0 or later**
- **MAUI** for cross-platform UI and AR components
- **WPF** for desktop-based AR and image recognition
- **WinUI** for modern Windows UI support

## Building and Running

### Prerequisites

Ensure you have the following installed:

- .NET SDK (8.0 or later)
- Visual Studio 2022 or later (with MAUI, WPF, and WinUI workloads installed)

### Steps to Build:

1. Clone the repository:
   ```bash
   git clone https://github.com/gachris/OpenVision.git
   ```

2. Open the solution in Visual Studio:
   ```bash
   cd OpenVision
   open OpenVision.sln
   ```

3. Restore dependencies:
   ```bash
   dotnet restore
   ```

4. Build the solution:
   ```bash
   dotnet build
   ```

## Usage

After building and running the project, you can access the following features:

- **Feature Extraction**: Detect and extract features from images.
- **Feature Matching**: Match features across multiple images using different algorithms.
- **Homography Calculation**: Perform homography for planar object detection.
- **AR Integration**: Integrate augmented reality features with camera input.

## Using NuGet Packages

You can also use **OpenVision** via NuGet packages instead of cloning and building the repository manually. This method simplifies integration into your project and ensures that you're always using the latest version.

### Steps to Install from NuGet:

1. In your project, open the **NuGet Package Manager** in Visual Studio:
   - Go to **Tools > NuGet Package Manager > Manage NuGet Packages for Solution**.

2. Search for the relevant **OpenVision** packages:
   - `OpenVision.Core` for core vision features.
   - `OpenVision.Maui` for MAUI-specific functionality.
   - `OpenVision.Wpf` for WPF-specific functionality.
   - `OpenVision.WinUI` for WinUI-specific functionality.

3. Install the desired packages by clicking the **Install** button.

Alternatively, you can use the **.NET CLI** to install the packages directly:

- For Core:
   ```bash
   dotnet add package OpenVision.Core
   ```

- For MAUI platform:
   ```bash
   dotnet add package OpenVision.Maui
   ```

- For WPF platform:
   ```bash
   dotnet add package OpenVision.Wpf
   ```

- For WinUI platform:
   ```bash
   dotnet add package OpenVision.WinUI
   ```

Once the packages are installed, you can access the same functionality, such as feature detection, matching, and AR integration, within your project without needing to clone the repository.

## Contributing

We welcome contributions! Please submit a pull request or raise an issue if you encounter any bugs or have suggestions for improvements.

### Steps for Contributing:

1. Fork the repository.
2. Create a feature branch:
   ```bash
   git checkout -b feature/new-feature
   ```
3. Commit your changes and push:
   ```bash
   git push origin feature/new-feature
   ```
4. Submit a pull request.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE.txt) file for details.
