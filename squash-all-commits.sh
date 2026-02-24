#!/bin/bash
# Squash all commits into one and remove previous history
# WARNING: This rewrites history. Use with caution, especially if you've already pushed.

git checkout --orphan temp_branch
git add -A
git commit -m "Initial commit"
git branch -D master
git branch -m master

echo "Done! All commits squashed into one. Use 'git push -f origin master' if you need to push."
