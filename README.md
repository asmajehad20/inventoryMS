# inventoryMS
An inventory management system CLLI application with APIs, designed to track and manage inventory products. It's used to keep track of product in inventoy mainly stock status, keep inventory product organized in categories.
some features are added in sight of future growth of project to add finantial tracking of products (eg. adding new roles, product price)

## Table of Contents

1. [Features](#features)
   ---
3. [Installation](#installation)
   ---
5. [Usage](#usage)
   ---

## Features
users in the systems are of roles user, admin.

role user can:
- add new product 
- update an existing product 
- delete product 
- search on product using a keyword. (keyword could be product name, barcode, category, stock status).
- view the stock status and quantity of product.
- view all the products
- view all products of a status(eg get all product that are low on stock)

role admin can:
- all users permissions 
- add/delete/view categories.
- add/delete roles.
- view all system users.


## Installation

Follow these steps to install and set up the inventory management system:
1. **Clone the Repository**
2. **Navigate to the Project Directory**
3. **Install Dependencies:**
   - dotnet add package Npgsql. This command installs the Npgsql package, which is the .NET data provider for PostgreSQL.
4. **Set Up PostgreSQL Database**
5. **Start the Application:**

## Usage
### CLI (Console Application)
1. **Main Screen (Login/Register Screen):**
   - l or login  (to login)
     if valid user will redirect the user to the valid page according to role.
   - r or register (to register)
3. **User Screen:**
   - u (to view options related to user) users options: all/info/delete
   - p (to view options related to products) products options: all/add/update/delete/search/status/p_status
5.  **Admin Screen:**
   - roles (to show roles options) all/add/delete.
   - category (to show categories options) all/add/delete.
   - u (to view options related to user) all/info/delete
   - p (to view options related to products) all/add/update/delete/search/status/p_status
   
### API (Asp.Net Core Web Api Application)
**Products endpoint**
- GET
- POST
- PUT
- DELETE
