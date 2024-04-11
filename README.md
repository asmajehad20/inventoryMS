# inventoryMS
An inventory management system CLLI application with APIs, designed to track and manage inventory products. It's used to keep track of product in inventoy mainly stock status, keep inventory product organized in categories.
some features are added in sight of future growth of project to add finantial tracking of products (eg. adding new roles, product price)

## Table of Contents

1. [Features](#features)
   ---
3. [Installation](#installation)
   ---
5. [Intefaces](#intefaces)
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
   cd path-to-inventoryMS\inventory
   start inventory.sln
4. **Install Dependencies:**
   - dotnet add package Npgsql. This command installs the Npgsql package, which is the .NET data provider for PostgreSQL.
5. **Set Up PostgreSQL Database**
6. **Start the Application:**
start inventory.sln

## Intefaces
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
All Methods need Authorization, auth method is basic auth using username, password
{{urlBase}} is http://localhost:5280

<!-- ............................................ -->
**Method:** `GET`
<!-- ............................................ -->
**Description:**
gets all products.
<!-- ............................................ -->
**Example:**
GET h{{urlBase}}/api/Products

<!-- ............................................ -->
**Method:** `GET`
<!-- ............................................ -->
**Description:**
serach on products.
<!-- ............................................ -->
**Parameters:**
`keyword` keyword is either a product name or barcode 
<!-- ............................................ -->
**Example:**
GET h{{urlBase}}/api/Products/keyword

<!-- ............................................ -->
**Method:** `POST`
<!-- ............................................ -->
**Description:**
adds a new product to database.
<!-- ............................................ -->
**Parameters:**
`Name` (string, required): name of product.

`Barcode` (string, required): barcode of product.

`Price` (int, required): price if product.

`Quantity` (int, required): item numbers of product.

`Status` (string, required): stock status of product.

`Category` (string, required): category of product.
<!-- ............................................ -->
**Example:**
POST {{urlBase}}/api/Products \
  -H "Content-Type: application/json" \
  -d '{
  
    "name":"product_name",
    "barcode":"222244442222",
    "price":"100",
    "quantity":"30",
    "status":"low on stock",
    "category":"category name"
}'

<!-- ............................................ -->
**Method:** `PUT`
<!-- ............................................ -->
**Description:**
updates product.
<!-- ............................................ -->
**Parameters:**
`keyword` keyword is either a product name or barcode 

`Name` (string, required): name of product.

`Barcode` (string, required): barcode of product.

`Price` (int, required): price if product.

`Quantity` (int, required): item numbers of product.

`Status` (string, required): stock status of product.

`Category` (string, required): category of product.
<!-- ............................................ -->
**Example:**
all Parameters left empty will not update there values.
in this example only will update the quantity and the status.

POST {{urlBase}}/api/Products \
  -H "Content-Type: application/json" \
  -d '{
  
    "name":"",
    "barcode":"",
    "price":"",
    "quantity":"200",
    "status":"in stock",
    "category":""
}'
<!-- ............................................ -->
**Method:** `DELETE`
<!-- ............................................ -->
**Description:**
delete products.
<!-- ............................................ -->
**Parameters:**
`keyword` keyword is either a product name or barcode 
<!-- ............................................ -->
**Example:**
Delete h{{urlBase}}/api/Products/keyword
