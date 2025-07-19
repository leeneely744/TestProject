# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Unity 3D project (Unity 6000.1.11f1) using the Universal Render Pipeline (URP). It's a basic Unity project setup with standard Unity packages and minimal custom code.

## Key Architecture

- **Unity Project Structure**: Standard Unity project with Assets/, Library/, ProjectSettings/, and Packages/ directories
- **Rendering Pipeline**: Uses Universal Render Pipeline (URP) with mobile and PC render pipeline assets
- **MCP Integration**: Includes MCP Unity settings (ProjectSettings/McpUnitySettings.json) with server configuration on port 8090
- **Input System**: Uses Unity's new Input System with InputSystem_Actions.inputactions configuration

## Development Commands

This is a Unity project, so development is primarily done through the Unity Editor. No specific build scripts or package managers are configured.

## Essential Files

- `ProjectSettings/ProjectVersion.txt`: Contains Unity version information
- `ProjectSettings/McpUnitySettings.json`: MCP server configuration
- `Assets/InputSystem_Actions.inputactions`: Input system configuration
- `Assets/Settings/`: URP render pipeline settings and volume profiles

## Project Structure

- `Assets/Scenes/`: Contains SampleScene.unity
- `Assets/Settings/`: URP configuration files and volume profiles
- `Assets/TutorialInfo/`: Basic Unity tutorial assets and scripts
- `Library/`: Unity build cache and temporary files
- `Packages/`: Unity package dependencies
- `ProjectSettings/`: Unity project configuration