HTTPServer
==========

A simple HTTP server written in C#.

####Currently supports:
- GET operation
- Content-Type header (html, txt, jpeg, still needs PDF)
- Header returns length of content
- Returns 200 status with page contents
- Returns 404 status codes with error message (no error page, yet)
- Doesn't allow accessing files outside of docroot directory

####Still needs to be implemented:
- Last-Modified header
- Ensure logging is thread safe
- Handle docroot command line option
- Look into persistent connections
