# PharmacyGO! V.2 — Database Management App

This is the database management tool for [PharmacyGO! V.2](https://github.com/LucyCheng111/PharmacyGO), a Unity-based educational game developed for Oregon State University's College of Pharmacy. This WinForms C# application allows the project partner to add, edit, and remove questions from the game without modifying the Unity project directly.

**[Play PharmacyGO! V.2 on itch.io](https://shiningn-osu.itch.io/pharmacy-go-v2)**

---

## Features

- **CRUD** — Create, read, update, and delete question sets in the database
- **Duplicate question button** — quickly create similar questions from an existing one
- **Bulk upload** — add multiple question sets at once from a `.csv` file
- **Image management** — add, delete, and replace images in the database's live view via the Edit tab

---

## How It Works

The application fetches `jsonTest.json` through the GitHub API, parses it into memory as a `QuestionSet` object, and allows users to make edits locally. Clicking **Save** or **Bulk Upload** serializes the object back to JSON and pushes a commit to the repo.

---

## Bulk Upload

1. **Download the Template:** [Template](./bulk_upload_template.csv.xlsx). *(Click to open/download)*
2. **Fill the Data:** Ensure you follow the column order strictly with the [tutorial](./Bulk%20Upload%20Tutorial.pdf).
3. **Run Upload:** Click the Bulk Upload button in the app and select your file. **The upload may take time if adding images**

| Column # | Content | Note |
|---|---|---|
| 1 | Question Text | The main prompt |
| 2 | Question Image | Local path or URL |
| 3–12 | Options 1–5 | Alternates between Text and Image (e.g. Opt1 Text, Opt1 Image) |
| 13 | Correct Index | Always `0` for PharmacyGO! |
| 14 | Difficulty | Integer (1–5) |
| 15 | Minigame | Card Match, Whack-a-Mole, or Slapjack |
| 16 | Module | Integer (e.g. `3`) |
| 17 | Body Locations | Comma-separated (e.g. `Heart, Lungs`) |

> The system will automatically skip any row where the Question Text matches an existing entry in the database.
> If a user wantss to duplicate more than 1 question set, press the Duplicate Question button in the Create tab and enter the
> quantity of dupes you'd like to produce.

---

## Question Set Structure

| Property | .NET 8 Type | Default | Purpose |
|---|---|---|---|
| Question | `string` | `""` | The prompt shown to the player |
| Options | `List` | `new()` | 1–5 items/answers. Index `0` is always correct |
| MinigameType | `string` | `"None"` | Triggers game logic for minigames |
| Locations | `int` | `0` | Bit-packed Module and Body Part flags |

- Bits 0–7 are Body Location flags (e.g. Heart = `16`, Lungs = `32`)
- Bits 8–12 are Module Numbers (shift right by 8)

Refer to `DataModels.cs` comments for full bitwise logic explanations.

---

## Prerequisites

- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) with the **.NET Desktop Development** workload, or VS Code with the C# Dev Kit
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- **Newtonsoft.Json 13.0.4** — restored automatically via NuGet when the project is built
- A **GitHub Personal Access Token (PAT)** with `repo` permissions — required for the app to fetch and save `jsonTest.json`

---

## Setup & Installation

1. Clone this repository:
```bash
git clone https://github.com/kyofyufufufufufufufu/test_database1.git
cd test_database1
```
2. Open `WinFormsApp1.csproj` in Visual Studio 2022
3. Visual Studio will automatically restore NuGet packages on first build — or run manually:
```bash
dotnet restore
```
4. Add your PAT token when prompted by the app
5. Press **Run** (or `F5`) to launch the app

---

## Related Repositories

| Repository | Description |
|---|---|
| [PharmacyGO Unity Project](https://github.com/LucyCheng111/PharmacyGO) | Main Unity game |
| [Database Management App](https://github.com/kyofyufufufufufufufu/test_database1) | This repository |

---

## Team

| Name | Area | GitHub | Email |
|---|---|---|---|
| Annmarie Geiger | Database Management | [@kyofyufufufufufufufu](https://github.com/kyofyufufufufufufufu) | geigerta@oregonstate.edu |
| Lucy Cheng | AI Development | [@LucyCheng111](https://github.com/LucyCheng111) | chengjuh@oregonstate.edu |
| Nick Shininger | Game Content Development, Minigames | [@shiningn-osu](https://github.com/shiningn-osu) | shiningn@oregonstate.edu |
| Jakob Poore | Level Design | [@poorej](https://github.com/poorej) | poorej@oregonstate.edu |
| Max Baker | Game Mechanics Development | [@Crimson-Ender](https://github.com/Crimson-Ender) | bakerm7@oregonstate.edu |

---

## Support & Handoff

This project was developed as a capstone at Oregon State University and is fully complete as of Version 2. For installation help, technical issues, or feature requests, submit a request via the [PharmacyGO Support & Feedback Form](https://forms.gle/NajcPYnEa8jS3CWN6). Submissions are monitored by the project manager and will be routed to the appropriate team or future development group.

---

## License

This project was developed in partnership with the OSU College of Pharmacy. All rights reserved.
