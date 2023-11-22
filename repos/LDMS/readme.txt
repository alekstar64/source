# Server Installation Guide
## Overview
This guide provides instructions for installing server-side components on Oracle Database 19c.
## Prerequisites

1. **Oracle Database:** Ensure that Oracle Database 19c is installed on the server.
2. **Privileges:** Ensure that you have the necessary privileges to create tables and database objects.
3. **Scripts:** Download the PL/SQL script file (`LDMS_TABLES.sql`) to the server.

## Installing the Database

1. **Create Tables:**
    - Run the script `LDMS_TABLES.sql` using SQL*Plus or a SQL script execution tool.
    - Example:
      ```bash
      sqlplus username/password@localhost @LDMS_TABLES.sql
      ```

## Testing

1. **Module Tests:**
    - Run module tests using SQL-Plus or a database tool such as DB Navigator.
    - Execute the test scripts provided in the `LDMS_TESTS.DOCX` file.

## Usage Examples

1. **Function Call Example:**
    - Provide an example of calling a function or procedure using SQL queries.

## Special Instructions
If you want to use REST API functionality to access data, make sure that you already have Oracle REST Data Services (ORDS) installed on your system. You will also need to grant access rights to the PKG_EMP_SRV package in RESTful Services. ### Windows
### Mac
## Troubleshooting

1. **Common Issues:**
- If you encounter problems at the stage of creating database tables and objects, make sure that you have user authorisation
- Be sure to follow the sequence of actions and instructions specified in the LDMS_TABLES.sql file. 
- If problems occur at the stage of testing execution, check the messages in the ERR_MSG table
2. **Logs:**
    - In case of problems, review the messages in the ERR_MSG table
## Additional Resources

- For additional support, refer to the Oracle 19c documentation or contact support team mailto:alekstar64@gmail.com .
