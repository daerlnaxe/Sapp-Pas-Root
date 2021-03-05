# Sapp Pas Root
- Plugin for launchbox - Change paths for platforms, games...
- .Net Core / WPF 

# Versions:
### 2.0.0.1 (Alpha)
- BugFix: Launchbox uses a 4.7.2 of Drawing.Common.dll DxTBoxCore uses 5.0.0. Recompilation of DxTBoxCore with a previous version of the Compatibility package (3.1.1) and installation of the nugget System.Drawing.Common for the plugin in 4.7.2 version.

### 2.0.0.0 (Alpha)
- Based on previous plugin created with Forms
- .Net Core/.Net Standard / WPF
- Migration methods for files added, this part is in testing.
- Use the lastet official plugin library of Launchbox.
- Use a hash calculator to verify files are same before to ask to the user what to do (if different) or pass.
- Use a hash calculator to verify if files are same after copy, except if size is null (fake files to have a library of games only based on material stuff).
- Can be used as Executable to simulate actions.

# Dependencies
- HashCalc (Hash Calculator)
- DxLocalTransf for copy core (testing) 
- DxTBoxCore (common wpf windows, some parts still in progress). Version compiled with 3.1.1 Compatibility package
- DxPaths - relative and hardware link makers (modification on algorithms because of previous bugs)
- Hermes - new system for log (still in progress and testing)

#
- 
