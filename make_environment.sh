#!/bin/bash

GORILLA_DIR=".local/share/Steam/steamapps/common/Gorilla Tag/"
REPO_DIR="Projects/FallMonke"

alacritty --working-directory "$REPO_DIR" -e nvim &
sleep .25 # Fix annoying crashing from broken plugin
alacritty --working-directory "$REPO_DIR" &
hyprctl dispatch layoutmsg setlayout master # https://github.com/zakk4223/hyprWorkspaceLayouts
