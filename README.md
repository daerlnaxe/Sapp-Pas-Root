# Sapp Pas Root
- Plugin for launchbox - Change paths for platforms, games...
- .Net Core / WPF 

# Versions:
### Alpha 2.0.1.0 (27/04/2021)

### 2.0.0.3 (Alpha)

### 2.0.0.1 (Alpha)

### 2.0.0.0 (Alpha)

# Dependencies
- HashCalc (Hash Calculator)
- DxLocalTransf for copy core (testing) 
- DxTBoxCore (common wpf windows, some parts still in progress). Version compiled with 3.1.1 Compatibility package
- DxPaths - relative and hardware link makers (modification on algorithms because of previous bugs)
- Hermes - new system for log (still in progress and testing)

# Todo
- [ ] Add a new mode for games paths 'Forced SubFolders', validity wrong if path point to the root for app folders and/or additionnal apps.
- [ ] When stop copy, say copy successfull.
- [ ] L'upgrade total ne se fait pas
- [ ] See if 100ms pause is useful
- [x] Modified starting folder for migration (2.0.0.4)
- [x] Remove 500ms pause on migration part. (2.0.0.4)
- [x] Correction for DxLocalTransf, there were an error for special char in the path (2.0.0.4)
