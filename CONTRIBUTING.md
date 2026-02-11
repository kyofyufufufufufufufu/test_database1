# Database Management System

### Database Structure

This is the test database for PharmacyGO!

This database requires the use of a Winforms application -- accessible through
a PAT token -- and have question sets added to the database's json file.

### Features
- Create, Read, Update, and Deletion (CRUD) of question sets in the database.
- A duplicate question button for easy creation of similar questions.
- A bulk upload feature that allows .csv file contents to be added to the database.
- Adding, deleting, and replacing images within the database's live view in the Edit tab of the WinForms application.

#### CRUD
##### How it works
The application fetches the json file (jsonTest.json) through the GitHub API,
which parses into memory as a QuestionSet object, and allows users to perform edits locally.

Clicking Save or Bulk Upload serializes the object back to JSON and pushes a commit to the repo.

#### Bulk Upload
The bulk upload feature allows multiple question sets to be added to the database at once. 

1. **Download the Template:** [Template](./bulk_upload_template.csv.xlsx). *(Click to open/download)*
2. **Fill the Data:** Ensure you follow the column order strictly with the [tutorial](./Bulk%20Upload%20Tutorial.pdf).
3. **Run Upload:** Click the Bulk Upload button in the app and select your file. **The upload may take time if adding images**

| Column # | Content | Note |
| :--- | :--- | :--- |
| 1 | Question Text | The main prompt. |
| 2 | Question Image | Local path or URL. |
| 3-12 | Options 1-5 | Alternates between Text and Image (e.g., Opt1 Text, Opt1 Image). |
| 13 | Correct Index | Always `0` for PharmacyGO! |
| 14 | Difficulty | Integer (1-5). |
| 15 | Minigame | `Card Match`, `Whack-a-Mole`, or `Slapjack`. |
| 16 | Module | Integer (e.g., `3`). |
| 17 | Body Locations | Comma-separated (e.g., `Heart, Lungs`). |

> **The system will automatically skip any row where the `Question Text` matches an entry already in the database.**

### Question Set Structure
| Propery  | .NET 8 Type | Default  | Purpose |
| ------------- | ------------- | ------------- | ------------- |
| Question | string  | ""  | The prompt shown to the player.  |
| Options | List<Option>  | new()  | 1-5 items/answers. **Index 0 is always the correct option.** |
| MinigameType  | string  | "None"  | Triggers game logic for minigames. |
| Locations  | int  | 0 | Bit-packed Module and Body Part flags.  |

* Bits 0-7 are Body Location flags (Example: Heart = 16, Lungs = 32)
* Bits 8-12 are Module Numbers (Shift right by 8)

### Prerequisites & Local Setup

To run this project, you need Visual Studio 2022 (with the .NET Desktop Development workload - .NET 8) or VS Code with the C# Dev Kit. You can have both if you work between these two programs.

#### Steps:
    Clone the Repository:
    
    Bash

    git clone https://github.com/kyofyufufufufufufufu/test_database1.git
    cd test_database1

    Install Dependencies:
The project uses Newtonsoft.Json for data handling.

Visual Studio should restore these automatically, but you can run:
    Bash

    dotnet restore

    Environment Setup: You will need a GitHub Personal Access Token (PAT)
    with repo permissions to allow the app to fetch and save the jsonTest.json file.



### Code Quality (Linters & Formatters)

Before submitting code, please ensure it follows the standard C# conventions.

    Format Code: Ensure your IDE is set to use EditorConfig settings included in the repo.

    Build Check: Run a clean build to ensure no new warnings or errors are introduced:
    Bash

    dotnet build

### Contribution Workflow
#### 1. Branching

Always create a new branch for your work. Do not commit directly to main.

    Feature: minigame-name

    Fix: bug-description

    Docs: update-csv-template

#### 2. Pull Requests (PRs)

When your feature is ready, open a PR against the main branch. Your PR description should include:

    -What was changed or added.

    -A screenshot of the UI (if applicable).

    -Anything that caused changes to the json database structure.

#### 3. Definition of Done (DoD)

A task is considered Done only when:

    -The code compiles without errors or null warnings.

    -Images upload correctly to the /images folder on GitHub.

    -The locations bitwise encoding logic is verified for the specific module.

    -The jsonTest.json file is successfully updated via the app.

#### 4. Code Review Expectations

    -Expect feedback within 24–48 hours.

### Reporting Bugs & Requests

    -Where: Open an Issue in the GitHub repository.

    -Details: Include steps to reproduce the bug, the CSV file used (if it was a bulk upload error), and the specific error message received in the MessageBox.

### Where to Ask for Help

If you run into issues with the GitHub API or the WinForms designer:

    Primary Contact: Reach out via our project Discord/Teams channel.
If you’re not a member of this channel, please contact geigerta@oregonstate.edu

    Documentation: Refer to the DataModels.cs comments for bitwise logic explanations.
