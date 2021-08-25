![Hedera-Banner](./assets/banner.png)

# Hedera

IoC scanner that supports only Windows at the moment.

## What is Hedera

Hedera is an open-source IoC ( Indicator of Compromise ) scanner that allows you to simplify the detection phase of an incident response straightforwardly. 

The primary purpose of this project is to speed up the IR detection phase using a structured form in which researchers or analysts can describe their once-developed detection methods and make them shareable with others.

The **Indicator Database File** is written in YAML format and allows you to simply describe the indicators you want to search.

This repository contains:

- **Hedera**: the client that perform scanning
- **HederaLib**: the core of the project that contains the api used by Hedera

## What is the IDBF

The **IDBF - Indicator Database File** is Hedera's file to perform searches on the endpoints. It is divided into two main sections: the **header** and the **body**.

### The info property

The **information** property represents the header section and is always written on top of the IDBF file. It is composed of the following fields:

- **guid**: yaml file unique identifier
- **author**: this field contains the real name or the nickname of the analyst that has written the IDBF file
- **date**: this field indicates the date when the IDBF file is written
- **modified**: last modification date
- **status**: yaml file configuration status. It could be `experimental` or `stable` 
- **description**: this field contains a short description of the IDBF file.

Example:

```yaml
---
information:
  guid: a224d312-c61e-4f45-ae85-9fc102e49e66
  author: Antonio Blescia
  date: 2020/08/03
  modified: 2021/08/23
  status: experimental
  description: |
    descrizione
    riga 1
    riga 2
    riga 3
```

### The indicators property

The **indicators** property represents the body section and could contains the following sub-properties: **registry**, **file**, **process** and **event**.

#### > **Registry sub-property**

The **registry** sub-property allows searching an IoC based on the registry indicator. 

Below the supported configurations:

```yaml
# Searches using regular expression a value name that matches to ^IsMRUEstablishedd?$ regex inside the HKEY_USERS\{{sid}}\Software\Microsoft\Windows NT\CurrentVersion\Windows registry.
# The {{sid}} is replaced with the user SID value at runtime
- type: exists
  guid: 16a97688-3df8-4e02-bce8-778d982d3e32
  base_key: HKEY_USERS
  key: '{{sid}}\Software\Microsoft\Windows NT\CurrentVersion\Windows'
  value_name_regex: ^IsMRUEstablishedd?$
  is_recursive: false
```

```yaml
# Searches using regular expression a value name that matches to ^Device$ regex and value data that matches to ^Microsoft.*$ regex inside the HKEY_USERS\{{sid}}\Software\Microsoft\Windows NT registry. In this case the recursive search is enabled.
# The {{sid}} is replaced with the user SID value at runtime 
- type: data_value_regex
  guid: 904de9b0-a4d7-4e39-8fd2-754324a4d29b
  base_key: HKEY_USERS
  key: '{{sid}}\Software\Microsoft\Windows NT'
  value_name_regex: ^Device$
  value_data_regex: ^Microsoft.*$
  is_recursive: true
```

#### > **File sub-property**

The **file** sub-property allows searching an IoC based on the file indicator.

```yaml
# Checks if a file exists in non-recursive mode
- type: exists
  guid: 1a438dba-bb7f-45d3-a8d2-fd56ab3e9126
  path: C:\Program Files\Mozilla Firefox\firefox.exe
  is_recursive: false
```

```yaml
# Checks if a file exists using regular expression with recursive mode 
# The {{user}} is replaced with the user name value at runtime

- type: exists
  guid: 42d17885-ce39-4b8b-a542-ac4751eaabc5
  path: C:\Users\{{user}}
  filename: ^file\.exe$
  is_recursive: true
```

```yaml
# Checks the SHA-256 hash value of a spacific file
- type: hash
  guid: 22560d93-197b-4488-8d6e-a780a2ae5067
  path: C:\Program Files\Mozilla Firefox\firefox.exe
  sha256_hash: F98155B06D845E3218949E8BD959DA4741FFCD2736F963A11C7BCB7230460279
  is_recursive: false
```

```yaml
# Checks the SHA-256 hash value of a file to find recursively
- type: hash
  guid: 07114ca5-d93c-47e3-abae-954fa23842a6
  path: C:\Program Files\Mozilla Firefox
  filename: ^firefox\.exe$
  sha256_hash: F98155B06D845E3218949E8BD959DA4741FFCD2736F963A11C7BCB7230460279
  is_recursive: true
```

```yaml
# Checks the IMPHASH hash value of a spacific file
- type: imphash
  guid: 53c88a83-33b8-49e5-b309-71cea51b8357
  path: C:\Program Files\Mozilla Firefox\firefox.exe
  value: C483AB042998E5D3F9AC1D5A7C7ABDB2
  is_recursive: false
```

```yaml
# Checks the IMPHASH hash value of a file to find recursively
- type: imphash
  path: C:\Program Files\Mozilla Firefox
  guid: 090dcd6d-faa6-4f3c-8353-aae7317a1ba9
  filename: ^firefox\.exe$
  value: C483AB042998E5D3F9AC1D5A7C7ABDB2
  is_recursive: true
```

```yaml
# Finds the exe files that have the IMPHASH value
# WARNING: this type of search could be very expensive 
- type: imphash
  path: C:\Program Files\Mozilla Firefox
  guid: 068028a4-2f18-4d85-adb7-960b922ceab7
  filename: *.exe
  value: C483AB042998E5D3F9AC1D5A7C7ABDB2
  is_recursive: true
```

```yaml
# Checks if the file satisfy the yara rule
- type: yara
  guid: 9e001cdd-5928-4c27-be33-c3dab057207c
  path: C:\test\documento.txt
  rule: C:\test\file.yara
  is_recursive: false
```

```yaml
# Finds all files that statisfy the yara rule in recursive mode.
# WARNING: this type of search could be very expensive 
- type: yara
  guid: c8dc388e-642c-4911-a7d7-c33fe7750baa
  path: C:\test
  filename: .*
  rule: C:\test\file.yara
  is_recursive: true
```

#### > **Process sub-property**

The **process** sub-property allows searching an IoC based on the process indicator.

```yaml
# Checks if there is a process named Calculator.exe in execution
- type: exists
  guid: 21174833-527a-46a9-afdd-eab618180e63
  name: Calculator.exe
```

```yaml
# Checks if there is a process named Calculator.exe with a specific hash in execution
- type: hash
  guid: f099098d-3831-4bdf-9304-9e14194cd0a1
  name: Calculator.exe
  sha256_hash: A29B233954BABDB6DE3FD512E22D0E152D188BCBF74F3B1F3AC9DE450007B769
```

#### > **Event sub-property**

The **event** sub-property allows searching an IoC based on the event indicator.

```yaml
# Checks if there are events with event id 4672 inside the security channel
- type: exists
  guid: 4957ef98-aa1d-4b65-953e-0ecb5662d58a
  log: security
  event_id: 4672
```

```yaml
# Checks if there are events with event id 1001 inside the application channel in a particula time range
- type: exists
  guid: 20b43436-ef85-44a8-8566-dca85a3b3855
  log: application
  event_id: 1001
  datetime_start: 15/12/2020 00:00:00
  datetime_end: 15/12/2020 23:59:00
  datetime_format: dd/MM/yyyy HH:mm:ss
```

## More IDBFs Examples

The [ioc.yaml](./Hedera/ioc.yaml) file contains a lot of examples that you can study and reuse. 

## Usage

To start a scan, you simply run `Hedera.exe <idbf file path>`. 

Without the idbf file path parameter, it will use the `ioc.yaml` file located into the same folder.

### Credits

This is a private project mainly developed by Antonio Blescia @ NoCommentLab with feedback from many fellow analysts and friends.
