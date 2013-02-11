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
- Handle docroot command line option
- Look into persistent connections
- Threading each individual request? (Not required, but it might help)

####Other features:
These are features that we don't need to complete for the final product, but are instead  
cool little novelty features we could have.  
- Create a cache for storing the files in memory.
- Configuration (INI) file for the HTTP server (it could load its default settings from this file)

###NOTE
Currently the server defaults to C:\www\ as its root folder. You will need to create this folder if it doesn't already exist.
