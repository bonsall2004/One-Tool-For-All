
# One Tool For All 

One tool for all is an open-source windows modification tool that allows you to edit hidden aspects of windows easily and safely, it works by editing registry components that are usually hidden behind group policy, which without Windows 10 / 11 Pro, you won't have.



## Features

- Hidden Windows Tweaks
- Multi-threading enabled for efficiency when using commands 


## FAQ

#### Does this work on Windows 11

Yes, this tool works on Windows 11, 10, 8.1 (Basically all windows versions with the windows registry). 


#### Is this safe?

Yes, the code has been tested for a long time, hours has been spent ensuring that everything works as it should do. Adding to that it is completely open-source and you are free to read through the code and compile it yourself if you still have doubts.




## Custom Tweaks

You can add your own custom tweaks for your install of the app by locating the TweakList.json file
```
%appdata%\Roaming\.otfa\TweaksList.json
```
This is an example tweak to show operating system information on the desktop (already included in the program by default)

```json

    "osInfo": {
      "codeName": "osInfo",
      "name": "OS Info",
      "description": "Display Operating System information on the desktop.",
      "safe": true,
      "category": "general",
      "version": "1.0.0",
      "userSetting": true,
      "getValues": {
        "warning": "Enabling this tweak will reload file explorer which can make your taskbar vanish for a few seconds or longer.",
        "command": [ "taskkill /f /im explorer.exe", "start explorer.exe" ],
        "scripts": [
          {
            "isRegistry": true,
            "keyLocation": "LocalMachine",
            "keyName": "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Windows\\",
            "keyPath": "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Windows\\",
            "valueName": "DisplayVersion",
            "keyType": "DWORD",
            "keyOffValue": "null",
            "keyOnValue": "1"
          }
        ]
      }
    },
```

I am very much aware that the json layout is abysmal and I plan on re-working it in the future but I have delayed releaseing this for too long now so make sure if you create your own tweaks that they're following this scheme, and to update them when the next version is released.
## Additional Information

Keep in mind that some windows versions do not have support for some of the tweaks, if a tweak doesn't work after a full restart of the computer then your OS version doesn't have support.


## Version History

### 1.0.0


## Authors

- [@bonsall2004](https://github.com/bonsall2004) Developer


## Helpers

- [Morgan](https://twitter.com/MorganHughes234) Icon Designer
- [@Okayrug93](https://github.com/Okayrug93) Bug Fixer
## License
[![GPLv3 License](https://img.shields.io/badge/License-GPL%20v3-yellow.svg)](https://www.gnu.org/licenses/gpl-3.0.en.html)

