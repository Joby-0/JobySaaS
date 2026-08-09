#!/bin/bash

set -e

echo "======================================"
echo "        Resetting SaaS Database"
echo "======================================"

PROJECT="DbContext"
STARTUP_PROJECT="AppWebApi"

echo ""
echo "1. Dropping database..."
dotnet ef database drop \
    --project "$PROJECT" \
    --startup-project "$STARTUP_PROJECT" \
    --force

echo ""
echo "2. Removing migrations..."

while dotnet ef migrations remove \
    --project "$PROJECT" \
    --startup-project "$STARTUP_PROJECT" \
    --force 2>/dev/null; do
    echo "Removed migration."
done

echo ""
echo "3. Creating InitialCreate migration..."

dotnet ef migrations add InitialCreate \
    --project "$PROJECT" \
    --startup-project "$STARTUP_PROJECT"

echo ""
echo "4. Updating database..."

dotnet ef database update \
    --project "$PROJECT" \
    --startup-project "$STARTUP_PROJECT"

echo ""
echo "======================================"
echo "        Database reset complete!"
echo "======================================"