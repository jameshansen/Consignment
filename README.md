# Consignment Manager

C# WinForms Application. Management program for retail stores that work on a consignment system (% shared on sale).

**Background**

I wrote this Consignment Management program for a Consignment Store located in Vancouver in 2011. I was employed by CompuMAX to develop this system. The CEO, Ken Tung, gave me permission to share the source code of the project publicly on my GitHub under a GPL license. Thank you Ken.

The project took 6-12 months to complete from start to finish. Work began in January 2011, with the first release to the customer in August 2011, followed by updates through to December 2011.

**Timeline**

| | |
|:--- |:--- |
| Jan 2011 | Work starts. The main window, the MySQL connection, and the purchase order and item entry forms. |
| Feb 2011 | Order entry logic, the Crystal Reports set, global error handling, and the consignment purchase desktop. |
| Mar to Jun 2011 | Data export and import, and the first of the sales reports. |
| Jul to Aug 2011 | More reports, consignment purging, and UI work. First release to the customer in August. |
| Nov to Dec 2011 | The consolidated consignor report, order date editing, and consignment payment recording. Development ends on 28 December. |
| 2013 | BC drops the HST and goes back to GST and PST, which leaves the 12% baked into the code wrong. |
| Jul 2026 | The source is recovered from backups, its history rebuilt commit by commit from them, and published under the GPL. |
| Aug 2026 | The browser demo. Windows XP, MySQL and the Crystal Reports runtime, installed unattended into a v86 virtual machine and served as a link. |
| Aug 2026 | Bug fixes and improvements. Buttons that crashed on an empty list, invoiced orders that stayed editable, deleted records that took a window down with them, and sales tax made configurable per item. |

**Live Online Demo**

The most interesting part of restoring and releasing this project was that enough time has passed since it's release in 2011 that modern PCs are able to run it inside a browser inside a Virtual Machine (VM) powered by the [v86](https://github.com/copy/v86) Emulator.

You can try the demo on the project page: https://jameshansen.ai/projects/consignment

Potentially this could be a way to bring more legacy Windows apps to the web without rewrites. To learn more about how this VM was created, check out the VM folder in this repository.

**Concept**

The concept is simple, a vendor provides items to a store to stock on their shelves. Upon the sale, the store pays the vendor a percentage of the revenue.

The program provides inventory and vendor management, consignment intake and sales, consignor payout tracking, and reporting.

**Capabilities**

- **Vendor and customer records** with lookup by customer code, first/last name, or phone.
- **Item-level inventory keyed to UPC/barcodes**, including printable **barcode item labels**.
- **An order status workflow**: open, pending, in progress, invoiced, work completed, cancelled.
- **Payment and payout tracking** to consignors (a payments ledger, net-payment-after-commission calculations, a cash log).
- **Configurable sales tax** via a table of tax codes, rates and icons, with each item carrying the code it is sold under.
- **A full Crystal Reports suite**: consignment agreement (the contract with the consignor), consignment invoice, sale receipt, daily sales, order detail, net/consolidated consignor payment reports, and a cash log report.
- **Data import/export** for moving the dataset in and out.

**Framework**

The framework is a little unconventional. While it is a Windows desktop application written in C#, MySQL was used for the database, rather than MSSQL, due to my familiarity at the time after working with MySQL in PHP.

**Requirements**

Crystal Reports Runtime is required which includes the required CrystalDecisions.* References - install the appropriate "CRRuntime" msi file for your system.

MySQL Connector .net is required - the original connector 6.3.4 installer is included here - mysql.data.msi

The database structure has to be prepopulated from consignment_db_structure.sql

For a demonstration dataset of fictional vendors, customers, consignments, sales and payments, load demo_database.sql afterwards. Its dates are generated relative to the day it is loaded, so the data always reads as recent.

Sales tax is set up in the Tax Codes window. The program creates and seeds the `CSTTBLTAX` table on startup, so an existing database needs no manual changes, and each code takes its icon from a bitmap in `icons/tax` beside the program.

**Important Notes**

This code is old, and is shared here for historical and informational purposes.

It's requirements and dependancies are old, which likely means the code is insecure and likely contains a number of CVEs. These will be addressed in due time.

Additionally the project is hard-coded for one store, in its printed reports and in the company details it puts on them.

Finally there are a number of plain-text MySQL queries in the source code which need to be replaced with LINQ or another safer method.

**Release**

Originally an installer was used that setup the database and installed all dependancies - this is not included in the Git repository.

There will not be a pre-built binary or release at this time until the above issues are addressed and I confirm it is safe to distribute.

**Screenshots**

![Consignment Manager screenshot 1](screens/consignment_01.png)

![Consignment Manager screenshot 2](screens/consignment_02.png)

![Consignment Manager screenshot 3](screens/consignment_03.png)

![Consignment Manager screenshot 4](screens/consignment_04.png)
