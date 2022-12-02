# AdventOfCode2022
Solutions for Advent of Code 2022

# Session Secret

Copy/paste is a bit slow, so I made the starter program automatically download and store the input data in a file.

Open the solution in Visual Studio,and  right-click any project (they all have the same user secret id). Choose `Manage User Secrets`

![image](https://user-images.githubusercontent.com/36554739/205204636-4f538bb4-a644-482f-a0df-2bc05ac452d1.png)

This should be the data stored in the secrets file:

```json
{
    "SessionCookie": "<COOKIE VALUE GOES HERE>"
}
```

You can get the cookie value by going to your browser's Developer Console on https://adventofcode.com, locating cookies, and copying the value of the cookie with the name `session`. It looks like the cookie is good for ~1 year, so you shouldn't have to worry about doing this more than once (per year).
