#!/usr/bin/env bash

runtimes=(
    "win-x64"
    "win-x86"
    "win-arm64"
    "osx-x64"
    "osx-arm64"
    "linux-x64"
    "linux-musl-x64"
    "linux-musl-arm64"
    "linux-arm"
    "linux-arm64"
)

apt-get update -y && apt-get install zip -y
mkdir -p "$CI_PROJECT_DIR/release"

for runtime in "${runtimes[@]}"; do
    echo "Publishing $rid"
    dotnet publish --self-contained -c Release /p:Version="$CI_COMMIT_TAG" -r "$runtime" -o "$CI_PROJECT_DIR/release/$runtime" "$CI_PROJECT_DIR/Flink/Flink.csproj"
    zip -r "$CI_PROJECT_DIR/release/flink-$CI_COMMIT_TAG-$runtime.zip" "$CI_PROJECT_DIR/release/$runtime/"
    
    sha256sum "$CI_PROJECT_DIR/release/flink-$CI_COMMIT_TAG-$runtime.zip" > "$CI_PROJECT_DIR/release/flink-$CI_COMMIT_TAG-$runtime.sha256"
    sha512sum "$CI_PROJECT_DIR/release/flink-$CI_COMMIT_TAG-$runtime.zip" > "$CI_PROJECT_DIR/release/flink-$CI_COMMIT_TAG-$runtime.sha512"
done