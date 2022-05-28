# Import/Export Tool for Language Manager
This is a tool for use with Suriyun's MMORPG kit.
Tested on v1.75d_2020.3.

## Basic setup:
Add the `Language Manager Import Export` component to your GameInstance
GameObject in your 00Init scene (or to whatever GameObject holds your
`Language Manager` component).

## Export to a CSV file:
* Enter the language key (for example: ENG) of the language you want to export
into the `Language Key` field.
* Click the `Export To File...` button.
* Choose the location to save your exported CSV.

## Import from a CSV file:
* Enter the language key (for example: ENG) of the language you want to import
into the `Language Key` field.
* Click the `Import From File...` button.
* Choose the CSV file you want to import.
* The language will now be added to the `Language List` field of the
`Language Manager` component.

## Other options:
* If you wish to use the JSON format instead of CSV, tick the box format
`Use Json Not Csv`.
* If you will use Excel for editing your CSV and wish to prepend the ' character
to strings that Excel will otherwise interpret as formulas, tick the
box for `Escape Excel Formulas`.
* If you wish to copy the CSV (or JSON) string directly, click the `Export` button
and it will output to the TextArea on the `Language Manager Import Export`
component rather than to a file.
* You can also import by pasting the CSV (or JSON) string into the TextArea
and then clicking `Import`.
