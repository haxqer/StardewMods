#!/bin/bash
set -e

# 请将此路径修改为你的星露谷物语Mods文件夹路径
MODS_DIR="$HOME/Library/Application Support/Steam/steamapps/common/Stardew Valley/Mods/modtools"

BUILD_DIR="bin/Release/net6.0"
MOD_NAME="modtools"

if [ ! -d "$BUILD_DIR" ]; then
  echo "Build output not found. Please run ./build.sh first."
  exit 1
fi

echo "Deploying to $MODS_DIR ..."
mkdir -p "$MODS_DIR"
cp $BUILD_DIR/*.dll "$MODS_DIR/"
cp modtools.csproj "$MODS_DIR/"
cp manifest.json "$MODS_DIR/" 2>/dev/null || true
cp -r $BUILD_DIR/* "$MODS_DIR/"

echo "Zipping release..."
ZIP_NAME="${MOD_NAME}.zip"
zip -j "$BUILD_DIR/$ZIP_NAME" "$BUILD_DIR/*"
echo "Deployed and zipped as $ZIP_NAME." 