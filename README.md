# csharp-sql-from-scratch

Learn to connect C# to a SQL Server database, one small step at a time.
Four sessions, same project. Each session adds exactly one idea.

No prior database experience needed. If you can run `dotnet run`, you can do this.

---

## What you need

Pick one editor — either works fine for this tutorial.

- **VS Code** (lighter, works everywhere) — https://code.visualstudio.com
  - Install the **C# Dev Kit** extension (Extensions panel → search "C# Dev Kit" → Install)
- **Visual Studio Community** (free, Windows only) — https://visualstudio.microsoft.com/vs/community/
  - During install, tick the **.NET desktop development** workload
  - You don't need the paid version. Community is free forever for learning and personal projects.

You also need:

- **.NET 10 SDK** — https://dotnet.microsoft.com/download
- **A SQL Server database** — from class, Docker, or a fresh install. See [Setup](#setup) — three options, pick one.

> **Using .NET 8 or 9?** The code works on those too. Change one line in
> `csharp-sql-from-scratch.csproj`:
>
> ```xml
> <TargetFramework>net10.0</TargetFramework>
> ```
>
> to `net8.0` or `net9.0`. Everything else stays the same.

---

## Setup

First, decide **how you'll run SQL Server.** Three options:

- **You already have SQL Server** (LocalDB, or Express) → [Option A](#option-a--you-already-have-sql-server)
- **You don't have it and don't want to install it** → [Option B — Docker](#option-b--docker-no-install)
- **You want the full install** → [Option C — install SQL Server Express](#option-c--install-sql-server-express)

Pick one, then continue from [step 2](#2-get-the-code).

---

### Option A — You already have SQL Server

#### Restore the demo database

Download `db/ParkingInfoSystem.bak` from this repo.

**In SSMS:**
1. Right-click **Databases** → **Restore Database…**
2. Select **Device** → click the **…** button → **Add** → browse to the `.bak` file → OK
3. Click **OK** to restore

**In DBeaver/ Your favorite client:**
Use your client's restore/import option and point it at the `.bak` file.
The database name inside the backup is `ParkingInfoSystem`.

Verify it worked:

```sql
SELECT * FROM ParkingInfoSystem.dbo.ParkingSlots;
```

You should see five rows (A-01, A-02, B-01, B-02, C-01).

> **Can't restore the .bak?** (version mismatch, permissions, etc.) Run the
> setup commands from **Option B step 3** against your own SQL Server — same
> `CREATE TABLE` and `INSERT`, just without the Docker part. Or ask your
> instructor for a copy of the database.

Then skip to [step 2](#2-get-the-code).

---

### Option B — Docker (no install)

If you don't have SQL Server and don't want to install it, run it in a Docker
container instead. You get a real SQL Server, no setup, no admin rights needed,
and you can delete it with one command when you're done.

#### 1. Install Docker

- **Windows / macOS:** https://www.docker.com/products/docker-desktop/
- **Linux (Debian/Ubuntu):** https://docs.docker.com/engine/install/

Verify it's running:

```bash
docker --version
docker ps
```

`docker ps` should print a header row, not an error. If it errors, Docker
Desktop isn't running — open it and wait for the whale icon to stop animating.

#### 2. Start SQL Server

From the repo folder, run:

```bash
docker run -d --name school-sql \
  -e "ACCEPT_EULA=Y" \
  -e "MSSQL_SA_PASSWORD=LocalDocker!Pass1" \
  -p 1433:1433 \
  mcr.microsoft.com/mssql/server:2022-latest
```

> **That password** is a throwaway for a local container. It satisfies SQL
> Server's complexity rule (8+ chars, upper, lower, digit, symbol). Use it
> exactly as written so the connection string below works.

First run downloads ~1.5 GB. Check it's up:

```bash
docker ps
```

You want a row with `school-sql` and status `Up`. If it says `Restarting`,
run `docker logs school-sql` and look for the error.

#### 3. Create the demo database

Create the database:

```bash
docker exec -i school-sql /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "LocalDocker!Pass1" -C -Q "IF DB_ID('ParkingInfoSystem') IS NULL CREATE DATABASE ParkingInfoSystem;"
```

Then the table and rows:

```bash
docker exec -i school-sql /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "LocalDocker!Pass1" -C -d ParkingInfoSystem -Q "
CREATE TABLE dbo.ParkingSlots (
    SlotId      INT IDENTITY PRIMARY KEY,
    SlotNumber  NVARCHAR(20)  NOT NULL,
    SlotType    NVARCHAR(20)  NOT NULL,
    Status      NVARCHAR(20)  NOT NULL,
    HourlyRate  DECIMAL(10,2) NOT NULL
);
INSERT INTO dbo.ParkingSlots (SlotNumber, SlotType, Status, HourlyRate) VALUES
    ('A-01', 'Compact', 'Available', 2.50),
    ('A-02', 'Compact', 'Occupied',  2.50),
    ('B-01', 'Regular', 'Available', 3.75),
    ('B-02', 'Regular', 'Occupied',  3.75),
    ('C-01', 'Large',   'Available', 5.00);
"
```

> Same command works in PowerShell, bash, zsh, and WSL — no `<` redirection
> involved.

Verify:

```bash
docker exec -it school-sql /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "LocalDocker!Pass1" -C -d ParkingInfoSystem -Q "SELECT * FROM dbo.ParkingSlots;"
```

You should see five rows.

> If `/opt/mssql-tools18/bin/sqlcmd` errors, try `/opt/mssql-tools/bin/sqlcmd` —
> the path moved between image versions.

#### 4. Your connection string

With Docker, your `Program.cs` connection string should be:

```
Server=localhost,1433;Database=ParkingInfoSystem;User Id=sa;Password=LocalDocker!Pass1;TrustServerCertificate=True;
```

#### Cleaning up

When you're done with the tutorial and want your disk space back:

```bash
docker stop school-sql
docker rm school-sql
```

That deletes the container and its data. To start over later, run the
`docker run` command again.

Then skip to [step 2](#2-get-the-code).

---

### Option C — Install SQL Server Express

If you'd rather install a real SQL Server on your machine:

1. Download **SQL Server Express** — https://www.microsoft.com/sql-server/sql-server-downloads
2. Run the installer, choose **Basic**, accept the defaults.
3. When it's done, install **SSMS** (SQL Server Management Studio) to manage it.
4. Then follow **Option A** to restore the `.bak`.

This is the most "real" setup and matches what you'll see in most C# jobs,
but it's also the heaviest install (~1.5 GB plus SSMS).

---

### 2. Get the code

```bash
git clone https://github.com/adgasa22/csharp-sql-from-scratch.git
cd csharp-sql-from-scratch
```

No git? go to https://github.com/adgasa22/csharp-sql-from-scratch.git Click **Code → Download ZIP** on GitHub.

### 3. Open the project

**In VS Code:** **File → Open Folder** → select the `csharp-sql-from-scratch`
folder (the one with the `.csproj` in it).

**In Visual Studio Community:** **File → Open → Project/Solution** → select
`csharp-sql-from-scratch.csproj`.

### 4. Set your connection string

Open `Program.cs`. Near the top you'll see:

```csharp
string connectionString =
    "Server=localhost,1433;Database=ParkingInfoSystem;User Id=sa;" +
    "Password=YourPassword;TrustServerCertificate=True;";
```

Replace it with **your** connection string. Where to find it:

- **Visual Studio:** View → SQL Server Object Explorer → right-click your database → Properties → Connection String
- **SSMS:** right-click your database → Properties → View connection properties
- **DBeaver:** right-click your connection → Edit Connection → the connection details are all there
- **Don't know?** Ask whoever set up your class database.

If you're using LocalDB (comes with Visual Studio), it looks like this:

```
Server=(localdb)\MSSQLLocalDB;Database=ParkingInfoSystem;Trusted_Connection=True;TrustServerCertificate=True;
```

> **If you chose Option B (Docker),** the connection string is already peresnt just update the password to your database

### 5. Run it

**VS Code** — in the terminal:

```bash
dotnet run
```

**Visual Studio Community** — press **F5**, or click the green ▶ button at the top.

You should see five rows printed. **That's Session 1 done.** If it fails,
see [When it doesn't work](#when-it-doesnt-work).

---

## The sessions

Each session is a **git tag**. To see a session's code:

```bash
git checkout s01-bare        # or any tag below
dotnet run
git checkout main            # back to the latest
```

In Visual Studio Community, use **Git → Manage Branches → Tags** instead.

If you downloaded a ZIP, you already have the latest. To get an earlier
session, click the tag name below on GitHub → **Code → Download ZIP**.

---

### Session 1 — `s01-bare`

**What it teaches:** how to open a connection, send a query, and read rows.

Open `Program.cs` and follow along. The key lines:

```csharp
using (SqlConnection connection = new SqlConnection(connectionString))
{
    connection.Open();

    SqlCommand query = new SqlCommand(sql, connection);

    using (SqlDataReader reader = query.ExecuteReader())
    {
        while (reader.Read())
        {
            Console.WriteLine("...", reader.GetInt32(0), reader.GetString(1), ...);
        }
    }
}
```

**What each piece does:**

- `SqlConnection` — your phone line to the database
- `connection.Open()` — actually dials
- `SqlCommand` — the sentence you want to say (your SQL)
- `ExecuteReader()` — send it, get rows back
- `reader.Read()` — move to the next row; returns `false` when done
- `reader.GetInt32(0)` — read column 0 as an int

**Why the numbers?** `GetInt32(0)` reads the **first** column in your `SELECT`,
`GetString(1)` the second, and so on. Column order matters. So does the type:
column 4 (`HourlyRate`) is a `DECIMAL`, so it's `GetDecimal(4)`. Get either
wrong and you'll get a confusing error — see the table at the bottom.

**Why `using`?** It closes the connection when the block ends, even if
something crashes. Forget it and your app leaks connections until it freezes.

---

### Session 2 — `s02-list`

```bash
git checkout s02-list
```

**What it teaches:** turning rows into objects.

In Session 1 you printed directly inside the reader loop. That works, but
you never *had* the data — you could only print it. Session 2 fixes that.

New file `Parking.cs`:

```csharp
public class Parking
{
    public int SlotId { get; set; }
    public string? SlotNumber { get; set; }
    public string? SlotType { get; set; }
    public string? Status { get; set; }
    public decimal HourlyRate { get; set; }
}
```

The reader loop now builds a list:

```csharp
List<Parking> parkings = new();

using (SqlDataReader reader = query.ExecuteReader())
{
    while (reader.Read())
    {
        parkings.Add(new Parking
        {
            SlotId     = reader.GetInt32(0),
            SlotNumber = reader.GetString(1),
            SlotType   = reader.GetString(2),
            Status     = reader.GetString(3),
            HourlyRate = reader.GetDecimal(4)
        });
    }
}

foreach (var p in parkings)
{
    Console.WriteLine("...", p.SlotId, p.SlotNumber, ...);
}
```

**What changed:** the same `reader.GetXxx(n)` calls, but now they fill an
object instead of printing. Same numbers, same order, same types — nothing
mysterious got added. The output is identical. That's the point.

**Why bother?** Because now you have the data. You can sort it, filter it,
pass it to another method, return it. You couldn't do any of that in Session 1.

---

### Session 3 — `s03-connection-class`

```bash
git checkout s03-connection-class
```

**What it teaches:** moving the connection string into one reusable class.

Try this first: add a second query to Session 2's code. You'll immediately
retype `new SqlConnection(connectionString); connection.Open();`. Then a third
query, and you retype it again. **That's the problem this session solves.**

New file `Db.cs`:

```csharp
public class Db : IDisposable
{
    public SqlConnection Connection { get; }

    public Db(string connectionString)
    {
        Connection = new SqlConnection(connectionString);
        Connection.Open();
    }

    public void Dispose()
    {
        Connection.Dispose();
    }
}
```

`Program.cs` now looks like:

```csharp
using (Db db = new Db(connectionString))
{
    SqlCommand query = new SqlCommand(sql, db.Connection);
    // ... rest of the code inside the block
}
```

**What changed:**

- The connection opens in the **constructor**. `new Db(...)` connects.
- `using (Db db = new Db(connectionString))` — same `using` syntax you already
  know. It just wraps the `Db` object instead of the `SqlConnection` directly.
- `db.Connection` is the `SqlConnection` you pass to `SqlCommand`.

**One gotcha:** notice the `try/catch` moved to wrap the `new Db(...)` line.
If the connection fails, the constructor throws. If that's outside a try,
your app crashes instead of printing the error.

**Why the connection string is still in `Program.cs`:** because in a real
project it should come from configuration (an `appsettings.json` file), not
from code. That's a later lesson. For now, one string in one place is fine.

---

### Session 4 — `s04-dapper`

```bash
git checkout s04-dapper
```

**What it teaches:** the same job in four lines.

Install Dapper:

```bash
dotnet add package Dapper
```
Personal note: Dapper is one of my favorite libraries in the whole .NET
ecosystem. It does one thing — turn rows into objects — and does it so well
that I reach for it in every project. Once you've written the reader
loop by hand (Sessions 1–3), you'll see why.

(In Visual Studio Community: right-click the project → **Manage NuGet
Packages** → search `Dapper` → **Install**.)

Now replace the entire reader loop in `Program.cs` with:

```csharp
using (Db db = new Db(connectionString))
{
    var parkings = db.Connection.Query<Parking>(
        @"SELECT SlotId, SlotNumber, SlotType, Status, HourlyRate
          FROM dbo.ParkingSlots;"
    ).ToList();

    foreach (var p in parkings)
    {
        Console.WriteLine("...", p.SlotId, ...);
    }
}
```

That's it. `Db.cs` and `Parking.cs` didn't change.

**Why is this so short?** Because Dapper writes the boring part for you — the
`while (reader.Read())` loop and the `reader.GetInt32(0)` calls. It does that
by matching SQL column names to property names.

**Why does it work?** Because in Session 2 you named `Parking`'s properties
`SlotId`, `SlotNumber`, etc. — exactly matching the column names. That wasn't
an accident. Dapper is cashing in the discipline you already learned.

**The one way Dapper bites:** if you rename a property or a column and forget
to update the other side, Dapper doesn't error — it silently leaves that
property `null` or `0`. If you see empty output, check the names match.

**What it doesn't do:** it doesn't teach you how a connection works, how a
reader works, or what `GetInt32(0)` vs `GetDecimal(4)` means. That's why
you did Sessions 1–3 first. Dapper is a shortcut, not a replacement for
understanding.

---

## When it doesn't work

| What you see | What it means |
|---|---|
| `The type or namespace name 'SqlConnection' could not be found` | Missing package. Run `dotnet add package Microsoft.Data.SqlClient` (Sessions 1–3) or `dotnet add package Dapper` (Session 4). |
| Red squiggles but the code looks fine | C# Dev Kit is still loading. Wait 30 seconds. Or you opened the wrong folder — **File → Open Folder** → the folder with the `.csproj`. |
| `A network-related or instance-specific error…` | Can't reach the server. Check the `Server=` part of your connection string. Is SQL Server running? |
| `Login failed for user 'sa'` | Wrong password or wrong user in the connection string. |
| `Cannot open database "ParkingInfoSystem" requested by the login` | Database not restored (Option A) or not created (Option B step 3). |
| `The certificate chain was issued by an authority that is not trusted` | Missing `TrustServerCertificate=True;` at the end of your connection string. |
| `Invalid object name 'dbo.ParkingSlots'` | Table doesn't exist, or you're connected to the wrong database. |
| `Invalid column name 'SlotId'` | Your `SELECT` names a column that doesn't exist. Check spelling. |
| `Specified cast is not valid` / `InvalidCastException` | You called the wrong `GetXxx()`. E.g. `GetInt32(4)` on a `DECIMAL`. Check the type and position. |
| Output is empty rows or `null` values | (Session 4) Column name doesn't match property name in `Parking.cs`. |
| The `.bak` won't restore | Version mismatch or permissions. Use Option B step 3 commands against your own server, or ask your instructor. |
| `docker: command not found` | Docker isn't installed. See Option B step 1. |
| `docker: Error response from daemon: ... port is already allocated` | Something else is using port 1433. Stop it, or change `-p 1434:1433` and update the port in your connection string to `1434`. |
| `Unable to load DLL...` on Linux/macOS | You need the Linux/macOS SQL Server client. See the package docs. |
| `NETSDK1045: The current .NET SDK does not support targeting .NET 10.0` | Your SDK is older than .NET 10. Either install .NET 10, or change `<TargetFramework>` in the `.csproj` to match what you have (`net8.0` or `net9.0`). |

---

## The four tags at a glance

| Tag | Files changed | Lines added | Lines removed |
|---|---|---|---|
| `s01-bare` | `Program.cs` | ~50 | 0 |
| `s02-list` | `Program.cs`, `Parking.cs` | ~10 | ~10 (the inline print) |
| `s03-connection-class` | `Program.cs`, `Db.cs` | ~15 | ~3 |
| `s04-dapper` | `Program.cs` | ~3 | ~20 (the whole reader loop) |

**Notice the shape:** Session 2 is a rewrite (same line count, different
content). Session 3 is a small refactor. Session 4 deletes more than it adds.
That's what real code changes look like — rarely bigger, just sharper.

---

## License

MIT. Use it however you want.
