-- Multi Express Consignment - demo dataset
--
-- Fictional vendors, customers, consignments, sales and payments for
-- demonstration purposes. Every name, address and phone number is invented.
--
-- Load the schema first, then this file:
--   mysql -u root -p < consignment_db_structure.sql
--   mysql -u root -p < demo_database.sql
--
-- Re-running this file is safe: it clears the tables before inserting.
-- Dates are generated relative to the day the file is loaded, so the demo
-- always shows recent activity, including sales dated today.

USE `consignment_db`;

SET SQL_MODE='NO_AUTO_VALUE_ON_ZERO';

-- Midnight today as a naive unix timestamp, matching how the program converts
-- DateTime to unix seconds (no timezone adjustment).
SET @day := DATEDIFF(CURDATE(),'1970-01-01') * 86400;

TRUNCATE TABLE `CSTITEM`;
TRUNCATE TABLE `CSTORDER`;
TRUNCATE TABLE `CSTPAYMENT`;
DELETE FROM `PSVEMAST`;
DELETE FROM `SFCUMAST`;

--
-- Vendors (consignors)
--

INSERT INTO `PSVEMAST` (`CMCUCODE`, `CMCUNAME`, `CMNAME1ST`, `CMNAMESUR`, `CMPHONE`, `CMFAX1`, `CMADD1`, `CMADD2`, `CMCITY`, `CMSTATE`, `CMPOST`, `CMCOUNTRY`) VALUES
('MAR-CHE-1042', '', 'Marguerite', 'Chen', '604 5550142', '', '1847 W 4th Ave', '', 'Vancouver', 'BC', 'V6B 0A0', 'Canada'),
('DAV-OKO-2318', '', 'David', 'Okonkwo', '604 5552318', '', '220 E Georgia St', 'Apt 12', 'Vancouver', 'BC', 'V5T 1B3', 'Canada'),
('PRI-RAG-3765', '', 'Priya', 'Raghunathan', '778 5553765', '', '6155 Fraser St', '', 'Vancouver', 'BC', 'V6J 2C6', 'Canada'),
('TOM-ALV-4189', '', 'Tomas', 'Alvarez', '604 5554189', '', '3390 Kingsway', 'Unit 7', 'Vancouver', 'BC', 'V5V 3E9', 'Canada'),
('ING-SOL-5273', '', 'Ingrid', 'Solberg', '604 5555273', '', '1122 Lonsdale Ave', '', 'North Vancouver', 'BC', 'V7M 4G2', 'Canada'),
('WEI-ZHA-6014', '', 'Wei', 'Zhang', '778 5556014', '', '4580 No 3 Rd', 'Suite 210', 'Richmond', 'BC', 'V6X 5H5', 'Canada'),
('CLA-BEA-7382', '', 'Claire', 'Beaulieu', '604 5557382', '', '2075 Commercial Dr', '', 'Vancouver', 'BC', 'V6P 6J8', 'Canada'),
('SAM-ADE-8125', '', 'Samuel', 'Adeyemi', '604 5558125', '', '715 Carnarvon St', '', 'New Westminster', 'BC', 'V3M 7K1', 'Canada'),
('HAN-KOB-9037', '', 'Hana', 'Kobayashi', '778 5559037', '', '938 Denman St', 'Apt 405', 'Vancouver', 'BC', 'V6B 8L4', 'Canada'),
('ROB-FIT-1560', '', 'Robert', 'Fitzgerald', '604 5551560', '', '12480 88th Ave', '', 'Surrey', 'BC', 'V3W 9M7', 'Canada'),
('AIS-MAH-2694', '', 'Aisha', 'Mahmoud', '604 5552694', '', '5601 Willow St', '', 'Vancouver', 'BC', 'V6J 0N0', 'Canada'),
('WES-3311', 'Westcoast Vintage Co.', 'Dana', 'Whitfield', '604 5553311', '604 5553312', '1030 Granville St', '2nd Floor', 'Vancouver', 'BC', 'V5V 1P3', 'Canada');

--
-- Customers
--

INSERT INTO `SFCUMAST` (`CMCUCODE`, `CMCUNAME`, `CMNAME1ST`, `CMNAMESUR`, `CMPHONE`, `CMFAX1`, `CMADD1`, `CMADD2`, `CMCITY`, `CMSTATE`, `CMPOST`, `CMCOUNTRY`) VALUES
('JEN-MOR-1103', '', 'Jennifer', 'Moreau', '604 5551103', '', '2288 Yew St', '', 'Vancouver', 'BC', 'V6G 5H5', 'Canada'),
('AND-PAT-1247', '', 'Andrew', 'Patel', '778 5551247', '', '8100 Ackroyd Rd', 'Apt 302', 'Richmond', 'BC', 'V6X 6J8', 'Canada'),
('SOF-RIV-1382', '', 'Sofia', 'Rivera', '604 5551382', '', '455 Cordova St E', '', 'Vancouver', 'BC', 'V5Z 7K1', 'Canada'),
('MIC-THO-1455', '', 'Michael', 'Thompson', '604 5551455', '', '1590 Marine Dr', '', 'West Vancouver', 'BC', 'V7T 8L4', 'Canada'),
('LIN-NGU-1509', '', 'Linh', 'Nguyen', '778 5551509', '', '3720 Victoria Dr', '', 'Vancouver', 'BC', 'V5T 9M7', 'Canada'),
('GRA-OSU-1633', '', 'Grace', 'Osullivan', '604 5551633', '', '205 Lakeshore Rd', 'Unit 4', 'Coquitlam', 'BC', 'V3K 0N0', 'Canada'),
('HEC-RAM-1704', '', 'Hector', 'Ramos', '604 5551704', '', '9333 University Cres', '', 'Burnaby', 'BC', 'V5H 1P3', 'Canada'),
('EMI-LAR-1826', '', 'Emily', 'Larsen', '778 5551826', '', '1401 Hornby St', 'Apt 1802', 'Vancouver', 'BC', 'V6E 2R6', 'Canada'),
('YUS-KAR-1948', '', 'Yusuf', 'Karim', '604 5551948', '', '13450 104th Ave', '', 'Surrey', 'BC', 'V3W 3S9', 'Canada'),
('REB-HOL-2071', '', 'Rebecca', 'Holloway', '604 5552071', '', '3055 W Broadway', '', 'Vancouver', 'BC', 'V6P 4T2', 'Canada'),
('JUN-PAR-2196', '', 'Jun', 'Park', '778 5552196', '', '6420 Sussex Ave', 'Apt 908', 'Burnaby', 'BC', 'V5H 5V5', 'Canada'),
('NAT-DUB-2214', '', 'Natalie', 'Dubois', '604 5552214', '', '877 Bute St', '', 'Vancouver', 'BC', 'V6B 6W8', 'Canada'),
('OLI-BRA-2338', '', 'Oliver', 'Bradshaw', '604 5552338', '', '20250 Lougheed Hwy', '', 'Maple Ridge', 'BC', 'V2X 7X1', 'Canada'),
('AMA-SIN-2467', '', 'Amara', 'Singh', '778 5552467', '', '1155 Pacific Blvd', 'Apt 611', 'Vancouver', 'BC', 'V6J 8Y4', 'Canada'),
('TOM-WHI-2590', '', 'Thomas', 'Whitaker', '604 5552590', '', '540 St Andrews Ave', '', 'North Vancouver', 'BC', 'V7M 9Z7', 'Canada');

--
-- Consignment items
--

INSERT INTO `CSTITEM` (`upc`, `consignment_code`, `order_number`, `vendor_code`, `customer_code`,
  `description`, `price_minimum`, `price_suggested`, `price_sale`, `share`, `share_type`,
  `status`, `consignment_status`, `date_received`, `date_expiry`, `date_sold`, `date_paid`,
  `desc_brand`, `desc_gender`, `desc_garment`, `desc_material`, `desc_colour`, `desc_size`) VALUES
(100001, '100001', 200010, 'MAR-CHE-1042', 'AMA-SIN-2467', 'Vince Wool Coat', 118.30, 182.00, 182.00, 60.00, 'percentage', 'sold', 'Invoiced', @day - 90*86400 + 10*3600, @day - 0*86400 + 10*3600, @day - 0*86400 + 15*3600, 11, 'Vince', 'Ladies', 'Wool Coat', 'Leather', 'Ivory', '6'),
(100002, '100001', 200003, 'MAR-CHE-1042', 'EMI-LAR-1826', 'Banana Republic Handbag', 155.35, 239.00, 239.00, 50.00, 'percentage', 'sold', 'Invoiced', @day - 90*86400 + 10*3600, @day - 0*86400 + 10*3600, @day - 44*86400 + 13*3600, 11, 'Banana Republic', 'Ladies', 'Handbag', 'Wool', 'Burgundy', 'One Size'),
(100003, '100001', 200010, 'MAR-CHE-1042', 'AMA-SIN-2467', 'Coach Cashmere Sweater', 79.30, 122.00, 97.60, 60.00, 'percentage', 'sold', 'Invoiced', @day - 90*86400 + 10*3600, @day - 0*86400 + 10*3600, @day - 0*86400 + 15*3600, 11, 'Coach', 'Ladies', 'Cashmere Sweater', 'Tweed', 'Black', '2'),
(100004, '100001', 200010, 'MAR-CHE-1042', 'AMA-SIN-2467', 'Filson Wool Coat', 147.55, 227.00, 227.00, 40.00, 'percentage', 'sold', 'Invoiced', @day - 90*86400 + 10*3600, @day - 0*86400 + 10*3600, @day - 0*86400 + 15*3600, 11, 'Filson', 'Ladies', 'Wool Coat', 'Nylon', 'Blush', '6'),
(100005, '100001', 200001, 'MAR-CHE-1042', 'JEN-MOR-1103', 'Burberry Wool Scarf', 35.75, 55.00, 55.00, 50.00, 'percentage', 'sold', 'Invoiced', @day - 90*86400 + 10*3600, @day - 0*86400 + 10*3600, @day - 58*86400 + 11*3600, 11, 'Burberry', 'Unisex', 'Wool Scarf', 'Polyester Blend', 'Rust', 'One Size'),
(100006, '100002', 200007, 'DAV-OKO-2318', 'OLI-BRA-2338', 'Burberry Trench Coat', 130.65, 201.00, 160.80, 50.00, 'percentage', 'sold', 'Work Completed', @day - 84*86400 + 10*3600, @day - -6*86400 + 10*3600, @day - 16*86400 + 11*3600, 12, 'Burberry', 'Ladies', 'Trench Coat', 'Merino Wool', 'Forest Green', '10'),
(100007, '100002', 200001, 'DAV-OKO-2318', 'JEN-MOR-1103', 'Frye Denim Jacket', 84.50, 130.00, 104.00, 50.00, 'percentage', 'sold', 'Work Completed', @day - 84*86400 + 10*3600, @day - -6*86400 + 10*3600, @day - 58*86400 + 11*3600, 12, 'Frye', 'Unisex', 'Denim Jacket', 'Polyester Blend', 'Cobalt', 'XS'),
(100008, '100002', 200005, 'DAV-OKO-2318', 'REB-HOL-2071', 'Marc Jacobs Denim Jacket', 76.70, 118.00, 118.00, 50.00, 'percentage', 'sold', 'Work Completed', @day - 84*86400 + 10*3600, @day - -6*86400 + 10*3600, @day - 30*86400 + 12*3600, 12, 'Marc Jacobs', 'Unisex', 'Denim Jacket', 'Tweed', 'Forest Green', 'XS'),
(100009, '100002', 0, 'DAV-OKO-2318', '', 'Aritzia Chinos', 48.75, 75.00, 0.00, 50.00, 'percentage', 'unsold', 'Work Completed', @day - 84*86400 + 10*3600, @day - -6*86400 + 10*3600, 0, 0, 'Aritzia', 'Mens', 'Chinos', 'Polyester Blend', 'Dove Grey', 'XS'),
(100010, '100003', 0, 'PRI-RAG-3765', '', 'Lululemon Down Parka', 135.85, 209.00, 0.00, 50.00, 'percentage', 'unsold', 'Open', @day - 78*86400 + 10*3600, @day - -12*86400 + 10*3600, 0, 0, 'Lululemon', 'Unisex', 'Down Parka', 'Nylon', 'Dove Grey', 'XS'),
(100011, '100003', 0, 'PRI-RAG-3765', '', 'Kate Spade Selvedge Jeans', 109.85, 169.00, 0.00, 50.00, 'percentage', 'unsold', 'Open', @day - 78*86400 + 10*3600, @day - -12*86400 + 10*3600, 0, 0, 'Kate Spade', 'Mens', 'Selvedge Jeans', 'Leather', 'Dove Grey', 'XL'),
(100012, '100003', 0, 'PRI-RAG-3765', '', 'Pendleton Handbag', 218.40, 336.00, 0.00, 168.00, 'value', 'unsold', 'Open', @day - 78*86400 + 10*3600, @day - -12*86400 + 10*3600, 0, 0, 'Pendleton', 'Ladies', 'Handbag', 'Merino Wool', 'Blush', 'One Size'),
(100013, '100003', 0, 'PRI-RAG-3765', '', 'Banana Republic Peacoat', 98.80, 152.00, 0.00, 50.00, 'percentage', 'unsold', 'Open', @day - 78*86400 + 10*3600, @day - -12*86400 + 10*3600, 0, 0, 'Banana Republic', 'Mens', 'Peacoat', 'Merino Wool', 'Camel', 'L'),
(100014, '100003', 0, 'PRI-RAG-3765', '', 'Frye Denim Jacket', 52.00, 80.00, 0.00, 40.00, 'percentage', 'unsold', 'Open', @day - 78*86400 + 10*3600, @day - -12*86400 + 10*3600, 0, 0, 'Frye', 'Unisex', 'Denim Jacket', 'Nylon', 'Blush', 'XL'),
(100015, '100003', 0, 'PRI-RAG-3765', '', 'Kate Spade Leather Satchel', 58.50, 90.00, 0.00, 40.00, 'percentage', 'unsold', 'Open', @day - 78*86400 + 10*3600, @day - -12*86400 + 10*3600, 0, 0, 'Kate Spade', 'Unisex', 'Leather Satchel', 'Merino Wool', 'Rust', 'One Size'),
(100016, '100004', 200006, 'TOM-ALV-4189', 'LIN-NGU-1509', 'Coach Cashmere Sweater', 104.00, 160.00, 160.00, 60.00, 'percentage', 'sold', 'In Progress', @day - 72*86400 + 10*3600, @day - -18*86400 + 10*3600, @day - 23*86400 + 14*3600, 0, 'Coach', 'Ladies', 'Cashmere Sweater', 'Suede', 'Blush', '10'),
(100017, '100004', 200002, 'TOM-ALV-4189', 'MIC-THO-1455', 'Hugo Boss Pencil Skirt', 33.80, 52.00, 52.00, 26.00, 'value', 'sold', 'In Progress', @day - 72*86400 + 10*3600, @day - -18*86400 + 10*3600, @day - 51*86400 + 15*3600, 0, 'Hugo Boss', 'Ladies', 'Pencil Skirt', 'Wool', 'Charcoal', '14'),
(100018, '100004', 0, 'TOM-ALV-4189', '', 'Filson Trench Coat', 74.75, 115.00, 0.00, 50.00, 'percentage', 'unsold', 'In Progress', @day - 72*86400 + 10*3600, @day - -18*86400 + 10*3600, 0, 0, 'Filson', 'Ladies', 'Trench Coat', 'Denim', 'Blush', '2'),
(100019, '100004', 0, 'TOM-ALV-4189', '', 'Roots Down Parka', 181.35, 279.00, 0.00, 60.00, 'percentage', 'unsold', 'In Progress', @day - 72*86400 + 10*3600, @day - -18*86400 + 10*3600, 0, 0, 'Roots', 'Unisex', 'Down Parka', 'Cashmere', 'Burgundy', 'S'),
(100020, '100004', 0, 'TOM-ALV-4189', '', 'Banana Republic Cardigan', 24.70, 38.00, 0.00, 50.00, 'percentage', 'unsold', 'In Progress', @day - 72*86400 + 10*3600, @day - -18*86400 + 10*3600, 0, 0, 'Banana Republic', 'Ladies', 'Cardigan', 'Silk', 'Navy', '6'),
(100021, '100005', 200005, 'ING-SOL-5273', 'REB-HOL-2071', 'Burberry Handbag', 237.90, 366.00, 366.00, 50.00, 'percentage', 'sold', 'Invoiced', @day - 66*86400 + 10*3600, @day - -24*86400 + 10*3600, @day - 30*86400 + 12*3600, 13, 'Burberry', 'Ladies', 'Handbag', 'Silk', 'Olive', 'One Size'),
(100022, '100005', 200005, 'ING-SOL-5273', 'REB-HOL-2071', 'Pendleton Wool Scarf', 29.90, 46.00, 46.00, 50.00, 'percentage', 'sold', 'Invoiced', @day - 66*86400 + 10*3600, @day - -24*86400 + 10*3600, @day - 30*86400 + 12*3600, 13, 'Pendleton', 'Unisex', 'Wool Scarf', 'Cotton', 'Ivory', 'One Size'),
(100023, '100005', 200002, 'ING-SOL-5273', 'MIC-THO-1455', 'Lululemon Leather Satchel', 137.15, 211.00, 211.00, 60.00, 'percentage', 'sold', 'Invoiced', @day - 66*86400 + 10*3600, @day - -24*86400 + 10*3600, @day - 51*86400 + 15*3600, 13, 'Lululemon', 'Unisex', 'Leather Satchel', 'Silk', 'Ivory', 'One Size'),
(100024, '100005', 200004, 'ING-SOL-5273', 'AND-PAT-1247', 'Club Monaco Selvedge Jeans', 84.50, 130.00, 104.00, 50.00, 'percentage', 'sold', 'Invoiced', @day - 66*86400 + 10*3600, @day - -24*86400 + 10*3600, @day - 37*86400 + 16*3600, 13, 'Club Monaco', 'Mens', 'Selvedge Jeans', 'Leather', 'Burgundy', 'XL'),
(100025, '100006', 200009, 'WEI-ZHA-6014', 'GRA-OSU-1633', 'Frye Wool Scarf', 35.75, 55.00, 55.00, 60.00, 'percentage', 'sold', 'Pending', @day - 60*86400 + 10*3600, @day - -30*86400 + 10*3600, @day - 0*86400 + 11*3600, 0, 'Frye', 'Unisex', 'Wool Scarf', 'Silk', 'Ivory', 'One Size'),
(100026, '100006', 0, 'WEI-ZHA-6014', '', 'Ted Baker Ankle Boots', 44.20, 68.00, 0.00, 50.00, 'percentage', 'unsold', 'Pending', @day - 60*86400 + 10*3600, @day - -30*86400 + 10*3600, 0, 0, 'Ted Baker', 'Ladies', 'Ankle Boots', 'Silk', 'Forest Green', '10'),
(100027, '100006', 0, 'WEI-ZHA-6014', '', 'Ted Baker Pencil Skirt', 31.85, 49.00, 0.00, 50.00, 'percentage', 'unsold', 'Pending', @day - 60*86400 + 10*3600, @day - -30*86400 + 10*3600, 0, 0, 'Ted Baker', 'Ladies', 'Pencil Skirt', 'Cashmere', 'Dove Grey', '2'),
(100028, '100006', 0, 'WEI-ZHA-6014', '', 'Lululemon Handbag', 173.55, 267.00, 0.00, 50.00, 'percentage', 'unsold', 'Pending', @day - 60*86400 + 10*3600, @day - -30*86400 + 10*3600, 0, 0, 'Lululemon', 'Ladies', 'Handbag', 'Polyester Blend', 'Rust', 'One Size'),
(100029, '100006', 0, 'WEI-ZHA-6014', '', 'Kate Spade Wrap Dress', 74.10, 114.00, 0.00, 40.00, 'percentage', 'unsold', 'Pending', @day - 60*86400 + 10*3600, @day - -30*86400 + 10*3600, 0, 0, 'Kate Spade', 'Ladies', 'Wrap Dress', 'Polyester Blend', 'Cobalt', '4'),
(100030, '100007', 0, 'CLA-BEA-7382', '', 'Banana Republic Down Parka', 101.40, 156.00, 0.00, 78.00, 'value', 'unsold', 'Open', @day - 54*86400 + 10*3600, @day - -36*86400 + 10*3600, 0, 0, 'Banana Republic', 'Unisex', 'Down Parka', 'Tweed', 'Navy', 'XL'),
(100031, '100007', 0, 'CLA-BEA-7382', '', 'Marc Jacobs Silk Blouse', 34.45, 53.00, 0.00, 50.00, 'percentage', 'unsold', 'Open', @day - 54*86400 + 10*3600, @day - -36*86400 + 10*3600, 0, 0, 'Marc Jacobs', 'Ladies', 'Silk Blouse', 'Silk', 'Cobalt', '10'),
(100032, '100007', 0, 'CLA-BEA-7382', '', 'Theory Wrap Dress', 62.40, 96.00, 0.00, 48.00, 'value', 'unsold', 'Open', @day - 54*86400 + 10*3600, @day - -36*86400 + 10*3600, 0, 0, 'Theory', 'Ladies', 'Wrap Dress', 'Cotton', 'Camel', '6'),
(100033, '100007', 0, 'CLA-BEA-7382', '', 'Burberry Wool Coat', 198.25, 305.00, 0.00, 60.00, 'percentage', 'unsold', 'Open', @day - 54*86400 + 10*3600, @day - -36*86400 + 10*3600, 0, 0, 'Burberry', 'Ladies', 'Wool Coat', 'Leather', 'Cobalt', '2'),
(100034, '100008', 200003, 'SAM-ADE-8125', 'EMI-LAR-1826', 'Coach Handbag', 93.60, 144.00, 144.00, 50.00, 'percentage', 'sold', 'Invoiced', @day - 48*86400 + 10*3600, @day - -42*86400 + 10*3600, @day - 44*86400 + 13*3600, 14, 'Coach', 'Ladies', 'Handbag', 'Cashmere', 'Rust', 'One Size'),
(100035, '100008', 200008, 'SAM-ADE-8125', 'JUN-PAR-2196', 'Roots Leather Satchel', 150.15, 231.00, 231.00, 50.00, 'percentage', 'sold', 'Invoiced', @day - 48*86400 + 10*3600, @day - -42*86400 + 10*3600, @day - 9*86400 + 17*3600, 14, 'Roots', 'Unisex', 'Leather Satchel', 'Silk', 'Dove Grey', 'One Size'),
(100036, '100008', 200004, 'SAM-ADE-8125', 'AND-PAT-1247', 'Roots Wool Coat', 111.15, 171.00, 171.00, 40.00, 'percentage', 'sold', 'Invoiced', @day - 48*86400 + 10*3600, @day - -42*86400 + 10*3600, @day - 37*86400 + 16*3600, 14, 'Roots', 'Ladies', 'Wool Coat', 'Tweed', 'Charcoal', '10'),
(100037, '100008', 200006, 'SAM-ADE-8125', 'LIN-NGU-1509', 'Theory Leather Jacket', 264.55, 407.00, 407.00, 203.50, 'value', 'sold', 'Invoiced', @day - 48*86400 + 10*3600, @day - -42*86400 + 10*3600, @day - 23*86400 + 14*3600, 14, 'Theory', 'Mens', 'Leather Jacket', 'Leather', 'Camel', 'L'),
(100038, '100008', 200001, 'SAM-ADE-8125', 'JEN-MOR-1103', 'Coach Peacoat', 130.65, 201.00, 201.00, 40.00, 'percentage', 'sold', 'Invoiced', @day - 48*86400 + 10*3600, @day - -42*86400 + 10*3600, @day - 58*86400 + 11*3600, 14, 'Coach', 'Mens', 'Peacoat', 'Nylon', 'Ivory', 'M'),
(100039, '100008', 200007, 'SAM-ADE-8125', 'OLI-BRA-2338', 'Kate Spade Down Parka', 201.50, 310.00, 310.00, 50.00, 'percentage', 'sold', 'Invoiced', @day - 48*86400 + 10*3600, @day - -42*86400 + 10*3600, @day - 16*86400 + 11*3600, 14, 'Kate Spade', 'Unisex', 'Down Parka', 'Denim', 'Navy', 'M'),
(100040, '100009', 200008, 'HAN-KOB-9037', 'JUN-PAR-2196', 'Theory Selvedge Jeans', 59.80, 92.00, 92.00, 46.00, 'value', 'sold', 'In Progress', @day - 42*86400 + 10*3600, @day - -48*86400 + 10*3600, @day - 9*86400 + 17*3600, 0, 'Theory', 'Mens', 'Selvedge Jeans', 'Silk', 'Charcoal', 'M'),
(100041, '100009', 200008, 'HAN-KOB-9037', 'JUN-PAR-2196', 'Hugo Boss Silk Blouse', 68.25, 105.00, 84.00, 50.00, 'percentage', 'sold', 'In Progress', @day - 42*86400 + 10*3600, @day - -48*86400 + 10*3600, @day - 9*86400 + 17*3600, 0, 'Hugo Boss', 'Ladies', 'Silk Blouse', 'Tweed', 'Ivory', '2'),
(100042, '100009', 200009, 'HAN-KOB-9037', 'GRA-OSU-1633', 'Pendleton Pencil Skirt', 37.05, 57.00, 45.60, 28.50, 'value', 'sold', 'In Progress', @day - 42*86400 + 10*3600, @day - -48*86400 + 10*3600, @day - 0*86400 + 11*3600, 0, 'Pendleton', 'Ladies', 'Pencil Skirt', 'Suede', 'Rust', '10'),
(100043, '100009', 0, 'HAN-KOB-9037', '', 'Marc Jacobs Handbag', 69.55, 107.00, 0.00, 50.00, 'percentage', 'unsold', 'In Progress', @day - 42*86400 + 10*3600, @day - -48*86400 + 10*3600, 0, 0, 'Marc Jacobs', 'Ladies', 'Handbag', 'Silk', 'Olive', 'One Size'),
(100044, '100009', 0, 'HAN-KOB-9037', '', 'Hugo Boss Cardigan', 38.35, 59.00, 0.00, 50.00, 'percentage', 'unsold', 'In Progress', @day - 42*86400 + 10*3600, @day - -48*86400 + 10*3600, 0, 0, 'Hugo Boss', 'Ladies', 'Cardigan', 'Suede', 'Olive', '2'),
(100045, '100010', 0, 'ROB-FIT-1560', '', 'Ted Baker Blazer', 117.00, 180.00, 0.00, 50.00, 'percentage', 'unsold', 'Cancelled', @day - 36*86400 + 10*3600, @day - -54*86400 + 10*3600, 0, 0, 'Ted Baker', 'Mens', 'Blazer', 'Wool', 'Olive', 'S'),
(100046, '100010', 0, 'ROB-FIT-1560', '', 'Club Monaco Wool Scarf', 15.60, 24.00, 0.00, 50.00, 'percentage', 'unsold', 'Cancelled', @day - 36*86400 + 10*3600, @day - -54*86400 + 10*3600, 0, 0, 'Club Monaco', 'Unisex', 'Wool Scarf', 'Cashmere', 'Forest Green', 'One Size'),
(100047, '100010', 0, 'ROB-FIT-1560', '', 'Herschel Peacoat', 106.60, 164.00, 0.00, 50.00, 'percentage', 'unsold', 'Cancelled', @day - 36*86400 + 10*3600, @day - -54*86400 + 10*3600, 0, 0, 'Herschel', 'Mens', 'Peacoat', 'Wool', 'Olive', 'L'),
(100048, '100011', 200003, 'AIS-MAH-2694', 'EMI-LAR-1826', 'Burberry Evening Dress', 183.95, 283.00, 283.00, 50.00, 'percentage', 'sold', 'Open', @day - 30*86400 + 10*3600, @day - -60*86400 + 10*3600, @day - 44*86400 + 13*3600, 0, 'Burberry', 'Ladies', 'Evening Dress', 'Denim', 'Dove Grey', '10'),
(100049, '100011', 0, 'AIS-MAH-2694', '', 'Pendleton Leather Jacket', 259.35, 399.00, 0.00, 40.00, 'percentage', 'unsold', 'Open', @day - 30*86400 + 10*3600, @day - -60*86400 + 10*3600, 0, 0, 'Pendleton', 'Mens', 'Leather Jacket', 'Wool', 'Charcoal', 'M'),
(100050, '100011', 0, 'AIS-MAH-2694', '', 'Ted Baker Handbag', 211.90, 326.00, 0.00, 50.00, 'percentage', 'unsold', 'Open', @day - 30*86400 + 10*3600, @day - -60*86400 + 10*3600, 0, 0, 'Ted Baker', 'Ladies', 'Handbag', 'Silk', 'Olive', 'One Size'),
(100051, '100011', 0, 'AIS-MAH-2694', '', 'Lululemon Selvedge Jeans', 83.20, 128.00, 0.00, 50.00, 'percentage', 'unsold', 'Open', @day - 30*86400 + 10*3600, @day - -60*86400 + 10*3600, 0, 0, 'Lululemon', 'Mens', 'Selvedge Jeans', 'Polyester Blend', 'Burgundy', 'M'),
(100052, '100011', 0, 'AIS-MAH-2694', '', 'Club Monaco Trench Coat', 63.05, 97.00, 0.00, 40.00, 'percentage', 'unsold', 'Open', @day - 30*86400 + 10*3600, @day - -60*86400 + 10*3600, 0, 0, 'Club Monaco', 'Ladies', 'Trench Coat', 'Cashmere', 'Blush', '12'),
(100053, '100011', 0, 'AIS-MAH-2694', '', 'Lululemon Trench Coat', 104.00, 160.00, 0.00, 60.00, 'percentage', 'unsold', 'Open', @day - 30*86400 + 10*3600, @day - -60*86400 + 10*3600, 0, 0, 'Lululemon', 'Ladies', 'Trench Coat', 'Linen', 'Forest Green', '12'),
(100054, '100012', 200009, 'WES-3311', 'GRA-OSU-1633', 'Theory Evening Dress', 94.90, 146.00, 146.00, 50.00, 'percentage', 'sold', 'Work Completed', @day - 24*86400 + 10*3600, @day - -66*86400 + 10*3600, @day - 0*86400 + 11*3600, 15, 'Theory', 'Ladies', 'Evening Dress', 'Wool', 'Navy', '6'),
(100055, '100012', 200007, 'WES-3311', 'OLI-BRA-2338', 'Aritzia Leather Satchel', 86.45, 133.00, 133.00, 50.00, 'percentage', 'sold', 'Work Completed', @day - 24*86400 + 10*3600, @day - -66*86400 + 10*3600, @day - 16*86400 + 11*3600, 15, 'Aritzia', 'Unisex', 'Leather Satchel', 'Cashmere', 'Camel', 'One Size'),
(100056, '100012', 200004, 'WES-3311', 'AND-PAT-1247', 'Cole Haan Cashmere Sweater', 57.20, 88.00, 70.40, 50.00, 'percentage', 'sold', 'Work Completed', @day - 24*86400 + 10*3600, @day - -66*86400 + 10*3600, @day - 37*86400 + 16*3600, 15, 'Cole Haan', 'Ladies', 'Cashmere Sweater', 'Wool', 'Olive', '6'),
(100057, '100012', 200006, 'WES-3311', 'LIN-NGU-1509', 'Roots Leather Satchel', 146.25, 225.00, 225.00, 50.00, 'percentage', 'sold', 'Work Completed', @day - 24*86400 + 10*3600, @day - -66*86400 + 10*3600, @day - 23*86400 + 14*3600, 15, 'Roots', 'Unisex', 'Leather Satchel', 'Wool', 'Navy', 'One Size'),
(100058, '100012', 200002, 'WES-3311', 'MIC-THO-1455', 'Diesel Trench Coat', 63.70, 98.00, 98.00, 40.00, 'percentage', 'sold', 'Work Completed', @day - 24*86400 + 10*3600, @day - -66*86400 + 10*3600, @day - 51*86400 + 15*3600, 15, 'Diesel', 'Ladies', 'Trench Coat', 'Cotton', 'Navy', '2'),
(100059, '100012', 0, 'WES-3311', '', 'Ted Baker Silk Blouse', 69.55, 107.00, 0.00, 50.00, 'percentage', 'unsold', 'Work Completed', @day - 24*86400 + 10*3600, @day - -66*86400 + 10*3600, 0, 0, 'Ted Baker', 'Ladies', 'Silk Blouse', 'Polyester Blend', 'Charcoal', '8'),
(100060, '100012', 0, 'WES-3311', '', 'Cole Haan Wool Coat', 89.05, 137.00, 0.00, 50.00, 'percentage', 'unsold', 'Work Completed', @day - 24*86400 + 10*3600, @day - -66*86400 + 10*3600, 0, 0, 'Cole Haan', 'Ladies', 'Wool Coat', 'Cashmere', 'Olive', '12');

--
-- Sale orders
--

INSERT INTO `CSTORDER` (`order_number`, `invoice_number`, `status`, `order_status`,
  `customer_code`, `customer_first_name`, `customer_last_name`, `date_order`, `items`, `total`) VALUES
(200001, '200001', 'invoice', 'Invoiced', 'JEN-MOR-1103', 'Jennifer', 'Moreau', @day - 58*86400 + 11*3600, 3, 403.20),
(200002, '200002', 'invoice', 'Invoiced', 'MIC-THO-1455', 'Michael', 'Thompson', @day - 51*86400 + 15*3600, 3, 404.32),
(200003, '200003', 'invoice', 'Invoiced', 'EMI-LAR-1826', 'Emily', 'Larsen', @day - 44*86400 + 13*3600, 3, 745.92),
(200004, '200004', 'invoice', 'Invoiced', 'AND-PAT-1247', 'Andrew', 'Patel', @day - 37*86400 + 16*3600, 3, 386.85),
(200005, '200005', 'invoice', 'Invoiced', 'REB-HOL-2071', 'Rebecca', 'Holloway', @day - 30*86400 + 12*3600, 3, 593.60),
(200006, '200006', 'invoice', 'Invoiced', 'LIN-NGU-1509', 'Linh', 'Nguyen', @day - 23*86400 + 14*3600, 3, 887.04),
(200007, '200007', 'invoice', 'Invoiced', 'OLI-BRA-2338', 'Oliver', 'Bradshaw', @day - 16*86400 + 11*3600, 3, 676.26),
(200008, '200008', 'invoice', 'Invoiced', 'JUN-PAR-2196', 'Jun', 'Park', @day - 9*86400 + 17*3600, 3, 455.84),
(200009, '200009', 'invoice', 'Invoiced', 'GRA-OSU-1633', 'Grace', 'Osullivan', @day - 0*86400 + 11*3600, 3, 276.19),
(200010, '200010', 'invoice', 'Invoiced', 'AMA-SIN-2467', 'Amara', 'Singh', @day - 0*86400 + 15*3600, 3, 567.39);

--
-- Payments: customer payments against sale orders, consignor payouts against
-- consignments. A row carries either an order_number or a consignment_code.
--

INSERT INTO `CSTPAYMENT` (`id`, `consignment_code`, `order_number`, `type`, `description`,
  `cn`, `expiry`, `date`, `amount`, `vendor_code`, `vendor_name`, `customer_code`, `customer_name`, `deleted`) VALUES
(1, NULL, '200001', 'CASH', 'Sale payment', '', '', @day - 58*86400 + 11*3600, 403.20, '', '', 'JEN-MOR-1103', 'Jennifer Moreau', 0),
(2, NULL, '200002', 'VISA', 'Sale payment', '9724', '03/28', @day - 51*86400 + 15*3600, 404.32, '', '', 'MIC-THO-1455', 'Michael Thompson', 0),
(3, NULL, '200003', 'DEBIT', 'Sale payment', '', '', @day - 44*86400 + 13*3600, 745.92, '', '', 'EMI-LAR-1826', 'Emily Larsen', 0),
(4, NULL, '200004', 'MASTERCARD', 'Sale payment', '1798', '02/31', @day - 37*86400 + 16*3600, 386.85, '', '', 'AND-PAT-1247', 'Andrew Patel', 0),
(5, NULL, '200005', 'CASH', 'Sale payment', '', '', @day - 30*86400 + 12*3600, 593.60, '', '', 'REB-HOL-2071', 'Rebecca Holloway', 0),
(6, NULL, '200006', 'VISA', 'Sale payment', '8504', '07/27', @day - 23*86400 + 14*3600, 887.04, '', '', 'LIN-NGU-1509', 'Linh Nguyen', 0),
(7, NULL, '200007', 'DEBIT', 'Sale payment', '', '', @day - 16*86400 + 11*3600, 676.26, '', '', 'OLI-BRA-2338', 'Oliver Bradshaw', 0),
(8, NULL, '200008', 'CHEQUE', 'Sale payment', '2154', '', @day - 9*86400 + 17*3600, 455.84, '', '', 'JUN-PAR-2196', 'Jun Park', 0),
(9, NULL, '200009', 'VISA', 'Sale payment', '5819', '12/29', @day - 0*86400 + 11*3600, 276.19, '', '', 'GRA-OSU-1633', 'Grace Osullivan', 0),
(10, NULL, '200010', 'CASH', 'Sale payment', '', '', @day - 0*86400 + 15*3600, 567.39, '', '', 'AMA-SIN-2467', 'Amara Singh', 0),
(11, '100001', NULL, 'CHEQUE', 'Consignor payout', '4111', '', @day - 20*86400 + 14*3600, 405.56, 'MAR-CHE-1042', 'Marguerite Chen', '', '', 0),
(12, '100002', NULL, 'CHEQUE', 'Consignor payout (partial)', '4112', '', @day - 26*86400 + 14*3600, 95.70, 'DAV-OKO-2318', 'David Okonkwo', '', '', 0),
(13, '100005', NULL, 'CHEQUE', 'Consignor payout', '4113', '', @day - 12*86400 + 14*3600, 384.60, 'ING-SOL-5273', 'Ingrid Solberg', '', '', 0),
(14, '100008', NULL, 'CHEQUE', 'Consignor payout', '4114', '', @day - 5*86400 + 14*3600, 694.80, 'SAM-ADE-8125', 'Samuel Adeyemi', '', '', 0),
(15, '100012', NULL, 'CHEQUE', 'Consignor payout (partial)', '4115', '', @day - 8*86400 + 14*3600, 195.84, 'WES-3311', 'Westcoast Vintage Co.', '', '', 0);

--
-- Continue numbering above the demo records. The program also resets these
-- from the [startcodes] section of settings.ini.
--

UPDATE `CSTITEM` SET `tax_code` = 'PG';
UPDATE `CSTITEM` SET `tax_rate` = 12.0000 WHERE `status` = 'sold';

ALTER TABLE `CSTITEM` AUTO_INCREMENT = 100062;
ALTER TABLE `CSTORDER` AUTO_INCREMENT = 200011;
ALTER TABLE `CSTPAYMENT` AUTO_INCREMENT = 16;
