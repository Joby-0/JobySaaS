#!/bin/bash

#Kör chmod +x database-rebuild-all.sh för att göra den körbar
#För att köra den, använd kommandot: ./database-rebuild-all.sh

set -e

# Move from _script/ to the solution root
cd "$(dirname "$0")/.."

echo "======================================"
echo "        Resetting SaaS Database"
echo "======================================"

PROJECT="DbContext/DbContext.csproj"
STARTUP_PROJECT="AppWebApi/AppWebApi.csproj"

echo ""
echo "Working directory:"
pwd

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
    --force 2>/dev/null
do
    echo "Migration removed."
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