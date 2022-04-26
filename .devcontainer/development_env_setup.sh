#!/bin/sh

echo "  1. Installing the needed CLI(s) Command Line Interfaces"

dotnet --info

echo "-------------------------------------------------------\n"

dotnet tool install --global dotnet-ef 

echo "-------------------------------------------------------\n"

dotnet build

echo "-------------------------------------------------------\n"

echo "  Done! Development can now begin :)"