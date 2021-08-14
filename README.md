![Hedera-Banner](./assets/banner.png)

# Hedera

IoC scanner for Windows (only at the time).

## What is Hedera

Hedera is an open source IoC ( Indicator of Compromise ) scanner that allows you to simplify the incident response phases in straightforward manner. It searches **registry**, **files**, **processes** and **events** indicators.

The main purpose of this project is to speed up the IR detection phase using a structured form in which researchers or analysts can describe their once developed detection methods and make them shareable with others.

The **Indicator Database File** is written in yaml format and allows you to describe in simple manner the indicators that you want to search.

## Use Cases

TODO 

## What is the IDBF

The **IDBF - Indicator Database File** is the file that Hedera uses to perform searches on the endpoints. It is divided in two main sections: the **header** and the **body**.

### The info property

The **info** property rappresents the header section and is always written on-top of the IDBF file. It is composed of the following fields:

- **author**: this field contains the real-name or the nickname of the analyst that has written the IDBF file
- **date**: this field indicates the date when the IDBF file is written
- **description**: this field contains a short description of the IDBF file.

Example:

```yaml
---
info:
  author: Antonio Blescia
  date: 08/03/2020
  description: >
    short description\n
    row 1\n
    row 2\n
    row 3
```

### The iocs property

The **iocs** property rappresents the body section and could contains the following sub-properties: **registry**, **file**, **process** and **event**.

#### > **Registry sub-property**

The **registry** sub-property allows to search an IoC based on registry indicator. 

Below the supported configurations:

```yaml
# Searches using regular expression a value name that matches to ^IsMRUEstablishedd?$ regex inside the HKEY_USERS\{{sid}}\Software\Microsoft\Windows NT\CurrentVersion\Windows registry.
# The {{sid}} is replaced with the user SID value at runtime
- type: exists
  base_key: HKEY_USERS
  key: '{{sid}}\Software\Microsoft\Windows NT\CurrentVersion\Windows'
  value_name_regex: ^IsMRUEstablishedd?$
  is_recursive: false
```

```yaml
# Searches using regular expression a value name that matches to ^Device$ regex and value data that matches to ^Microsoft.*$ regex inside the HKEY_USERS\{{sid}}\Software\Microsoft\Windows NT registry. In this case the recursive search is enabled.
# The {{sid}} is replaced with the user SID value at runtime 
- type: data_value_regex
  base_key: HKEY_USERS
  key: '{{sid}}\Software\Microsoft\Windows NT'
  value_name_regex: ^Device$
  value_data_regex: ^Microsoft.*$
  is_recursive: true
```

#### > **File sub-property**

The **file** sub-property allows to search an IoC based on file indicator.

```yaml
# Checks if a file exists in non-recursive mode
- type: exists
  path: C:\Program Files\Mozilla Firefox\firefox.exe
  is_recursive: false
```

```yaml
# Checks if a file exists using regular expression with recursive mode 
# The {{user}} is replaced with the user name value at runtime

- type: exists
  path: C:\Users\{{user}}
  filename: ^file\.exe$
  is_recursive: true
```

```yaml
# Checks the SHA-256 hash value of a spacific file
- type: hash
  path: C:\Program Files\Mozilla Firefox\firefox.exe
  sha256_hash: F98155B06D845E3218949E8BD959DA4741FFCD2736F963A11C7BCB7230460279
  is_recursive: false
```

```yaml
# Checks the SHA-256 hash value of a file to find recursively
- type: imphash
  path: C:\Program Files\Mozilla Firefox
  filename: ^firefox\.exe$
  sha256_hash: F98155B06D845E3218949E8BD959DA4741FFCD2736F963A11C7BCB7230460279
  is_recursive: true
```

```yaml
# Checks the IMPHASH hash value of a spacific file
- type: imphash
  path: C:\Program Files\Mozilla Firefox\firefox.exe
  value: C483AB042998E5D3F9AC1D5A7C7ABDB2
  is_recursive: false
```

```yaml
# Checks the IMPHASH hash value of a file to find recursively
- type: imphash
  path: C:\Program Files\Mozilla Firefox
  filename: ^firefox\.exe$
  value: C483AB042998E5D3F9AC1D5A7C7ABDB2
  is_recursive: true
```

#### > **Process sub-property**

TODO

#### > **Event sub-property**

TODO

## More IDBFs Examples

TODO

## Usage


### Credits

This is a private project mainly developed by Antonio Blescia @ NoCommentLab with feedback from many fellow analysts and friends.
