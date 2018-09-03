# iConfess [![Build Status](https://travis-ci.org/redplane/iConfession.svg?branch=master)](https://travis-ci.org/redplane/iConfession)

## Description:
- A small web application that allows users to view confessions of another user, also allows authenticated user make a confession and post it to the web.
- Allows administrators to manage users, posts and comments, ...

## Project structure:
- `API Service`: Which allows client apps to connect and consume its data.
- `Client web app`: A web application that user can use to watch and make confessions.

## Technology:
- `API Service`:
    - .Net core 2.1
    - Entity framework core 2.1
    - SQL Server
- `Client web app`:
    - AngularJS
    - Webpack 3

