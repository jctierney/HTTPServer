HTTPServer
==========

A simple HTTP server written in C#.

####Currently supports:
- GET operation
- Content-Type header (html, txt, jpeg, still needs PDF)
- Header returns length of content
- Last-Modified Header
- Returns 200 status with page contents
- Returns 404 status codes with error message (no error page, yet)
- Doesn't allow accessing files outside of docroot directory

####Still needs to be implemented:
- Ensure logging is thread safe
- ~~Handle docroot command line option~~
- Look into persistent connections
- ~~Threading each individual request? (Multiple threads)~~

####Other features:
These are features that we don't need to complete for the final product, but are instead  
cool little novelty features we could have.  
- Create a cache for storing the files in memory.
- Configuration (INI) file for the HTTP server (it could load its default settings from this file)

###Usage:
```
-p		specify the server port
-docroot	specify the root directory for server documents
-logfile	specify the log file
```
