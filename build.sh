#!/bin/bash
set -e

echo "Cleaning..."
dotnet clean

echo "Restoring..."
dotnet restore

echo "Building..."
dotnet build -c Release

echo "Build complete. Output in bin/Release/net6.0/" 