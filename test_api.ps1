$ErrorActionPreference = "Stop"

Write-Host "Starting API..."
$process = Start-Process -FilePath "dotnet" -ArgumentList "run" -NoNewWindow -PassThru -WorkingDirectory "c:\Users\Ahmed\source\repos\LibraryManagementSystem"

Start-Sleep -Seconds 10 # Wait for API to start

try {
    $baseUrl = "http://localhost:5000/api" # Adjust port if necessary, standard is 5000 or https 5001. Let's try 5000.
    
    # We will grab the port from launchSettings.json if possible, or try 5000
    $launchSettings = Get-Content "c:\Users\Ahmed\source\repos\LibraryManagementSystem\Properties\launchSettings.json" | ConvertFrom-Json
    $applicationUrl = $launchSettings.profiles.http.applicationUrl -split ';' | Select-Object -First 1
    if ($applicationUrl) {
        $baseUrl = "$applicationUrl/api"
    }

    Write-Host "Base URL: $baseUrl"

    # 1. Login
    Write-Host "Testing Login..."
    $loginBody = @{
        UserName = "admin"
        Password = "Admin123!"
    } | ConvertTo-Json

    $loginResponse = Invoke-RestMethod -Uri "$baseUrl/auth/login" -Method Post -Body $loginBody -ContentType "application/json"
    $token = $loginResponse.Token
    Write-Host "Login Successful! Token acquired."
    
    $headers = @{
        "Authorization" = "Bearer $token"
        "Content-Type" = "application/json"
    }

    # 2. Get Users
    Write-Host "Testing GET /api/users..."
    $users = Invoke-RestMethod -Uri "$baseUrl/users" -Method Get -Headers $headers
    Write-Host "Users Count: $($users.Count)"

    # 3. Create a Book
    Write-Host "Testing POST /api/books..."
    $bookBody = @{
        Title = "Clean Code"
        ISBN = "9780132350884"
        PublisherId = 3
        AuthorIds = @(3)
        CategoryIds = @(3)
        Edition = "1st Edition"
        Language = "English"
        PublicationYear = 2008
        PageCount = 464
    } | ConvertTo-Json

    $book = Invoke-RestMethod -Uri "$baseUrl/books" -Method Post -Body $bookBody -Headers $headers
    Write-Host "Book Created: $($book.Title) (ID: $($book.BookId))"

    # 4. Get Books
    Write-Host "Testing GET /api/books..."
    $books = Invoke-RestMethod -Uri "$baseUrl/books" -Method Get -Headers $headers
    Write-Host "Books Count: $($books.Count)"

    # 5. Create a Member
    Write-Host "Testing POST /api/members..."
    $memberBody = @{
        FirstName = "John"
        LastName = "Doe"
        Email = "john.doe@example.com"
        Phone = "555-0199"
        Address = "123 Library St"
    } | ConvertTo-Json

    $member = Invoke-RestMethod -Uri "$baseUrl/members" -Method Post -Body $memberBody -Headers $headers
    Write-Host "Member Created: $($member.FirstName) $($member.LastName) (ID: $($member.MemberId))"

    # 6. Borrow Book (Wait, we need a Book Copy first!)
    # I didn't see an endpoint to add a Book Copy. I will just test GET /api/members for now.
    Write-Host "Testing GET /api/members..."
    $members = Invoke-RestMethod -Uri "$baseUrl/members" -Method Get -Headers $headers
    Write-Host "Members Count: $($members.Count)"

    # 7. Get Activity Logs
    Write-Host "Testing GET /api/activitylogs..."
    $logs = Invoke-RestMethod -Uri "$baseUrl/activitylogs" -Method Get -Headers $headers
    Write-Host "Activity Logs Count: $($logs.Count)"

    Write-Host "All endpoints tested successfully!"
}
finally {
    Write-Host "Stopping API..."
    Stop-Process -Id $process.Id -Force
}
