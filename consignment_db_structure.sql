-- phpMyAdmin SQL Dump
-- version 3.2.0.1
-- http://www.phpmyadmin.net
--
-- Host: localhost
-- Generation Time: Feb 02, 2011 at 01:56 PM
-- Server version: 5.1.37
-- PHP Version: 5.2.10-2ubuntu6.7

SET SQL_MODE="NO_AUTO_VALUE_ON_ZERO";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Database: `consignment_db`
--

-- --------------------------------------------------------

--
-- Table structure for table `CSTITEM`
--

CREATE DATABASE consignment_db;
USE consignment_db;

CREATE TABLE IF NOT EXISTS `CSTITEM` (
  `upc` int(11) NOT NULL AUTO_INCREMENT,
  `consignment_code` varchar(20) NOT NULL,
  `order_number` int(11) NOT NULL,
  `vendor_code` varchar(20) NOT NULL,
  `customer_code` varchar(20) NOT NULL,
  `description` varchar(50) NOT NULL,
  `price_minimum` decimal(16,4) NOT NULL,
  `price_suggested` decimal(16,4) NOT NULL,
  `price_sale` decimal(16,4) NOT NULL,
  `share` decimal(16,4) NOT NULL,
  `share_type` enum('value','percentage') NOT NULL,
  `status` enum('unsold','sold') NOT NULL,
  `consignment_status` text NOT NULL,
  `date_received` bigint(20) NOT NULL,
  `date_expiry` bigint(20) NOT NULL,
  `date_sold` bigint(20) NOT NULL,
  `date_paid` bigint(20) NOT NULL,
  `desc_brand` text NOT NULL,
  `desc_gender` text NOT NULL,
  `desc_garment` text NOT NULL,
  `desc_material` text NOT NULL,
  `desc_colour` text NOT NULL,
  `desc_size` text NOT NULL,
  `tax_code` varchar(2) NOT NULL DEFAULT '',
  `tax_rate` decimal(7,4) NOT NULL DEFAULT '0.0000',
  PRIMARY KEY (`upc`)
) ENGINE=MyISAM  DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `CSTTBLTAX`
--

CREATE TABLE IF NOT EXISTS `CSTTBLTAX` (
  `tax_code` varchar(2) NOT NULL,
  `tax_desc` varchar(30) NOT NULL DEFAULT '',
  `tax_rate` decimal(7,4) NOT NULL DEFAULT '0.0000',
  `tax_icon` varchar(8) NOT NULL DEFAULT '',
  PRIMARY KEY (`tax_code`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

INSERT INTO `CSTTBLTAX` (`tax_code`, `tax_desc`, `tax_rate`, `tax_icon`) VALUES
('PG', 'PST AND GST', 12.0000, 'METAX1'),
('P', 'PST ONLY', 7.0000, 'METAX2'),
('G', 'GST ONLY', 5.0000, 'METAX3'),
('NO', 'NO TAX', 0.0000, 'METAX4'),
('H', 'HST', 12.0000, 'METAX9');

-- --------------------------------------------------------

--
-- Table structure for table `CSTORDER`
--

CREATE TABLE IF NOT EXISTS `CSTORDER` (
  `order_number` int(11) NOT NULL AUTO_INCREMENT,
  `invoice_number` text NOT NULL,
  `status` text NOT NULL,
  `order_status` text NOT NULL,
  `customer_code` varchar(20) NOT NULL,
  `customer_first_name` text NOT NULL,
  `customer_last_name` text NOT NULL,
  `date_order` bigint(20) NOT NULL,
  `items` int(11) NOT NULL,
  `total` decimal(16,4) NOT NULL,
  PRIMARY KEY (`order_number`)
) ENGINE=MyISAM  DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `CSTPAYMENT`
--

CREATE TABLE IF NOT EXISTS `CSTPAYMENT` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `consignment_code` varchar(20) DEFAULT NULL,
  `order_number` varchar(20) DEFAULT NULL,
  `type` text NOT NULL,
  `description` text NOT NULL,
  `cn` text NOT NULL,
  `expiry` text NOT NULL,
  `date` bigint(20) NOT NULL,
  `amount` decimal(16,4) NOT NULL,
  `vendor_code` text NOT NULL,
  `vendor_name` text NOT NULL,
  `customer_code` text NOT NULL,
  `customer_name` text NOT NULL,
  `deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM  DEFAULT CHARSET=latin1;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;

-- --------------------------------------------------------

--
-- Table structure for table `PSVEMAST`
--

CREATE TABLE IF NOT EXISTS `PSVEMAST` (
  `CMCUCODE` text,
  `CMCUNAME` text,
  `CMADD1` text,
  `CMADD2` text,
  `CMPOST` text,
  `CMPHONE` text,
  `CMFAX1` text,
  `CMORDERAMT` decimal(15,2) DEFAULT NULL,
  `CMAVGPAY` decimal(15,1) DEFAULT NULL,
  `CMCREDLIMT` int(11) DEFAULT NULL,
  `CMYTDSALES` decimal(15,2) DEFAULT NULL,
  `CMYTDINCNT` int(11) DEFAULT NULL,
  `CMLYRSALE` decimal(15,2) DEFAULT NULL,
  `CMLYRINCNT` int(11) DEFAULT NULL,
  `CMSTATSW` text,
  `CMINTERSW` text,
  `CMBALANCE` decimal(15,2) DEFAULT NULL,
  `CMACCODE` text,
  `CMDEPT` text,
  `CMDISCOUNT` decimal(15,2) DEFAULT NULL,
  `CMDISCDAY` int(11) DEFAULT NULL,
  `CMNETDAY` int(11) DEFAULT NULL,
  `CMTITLE` text,
  `CMNAME1ST` text,
  `CMNAMESUR` text,
  `CMTELEXT1` text,
  `CMCITY` text,
  `CMSTATE` text,
  `CMCOUNTRY` text,
  `CMMEMO1` text,
  `CMFOB` text,
  `CMVIA` text,
  `CMSALESMAN` text,
  `CMTAXNUM1` text,
  `CMTAXNUM2` text,
  `CMTERMDESC` text,
  `CMGLGROUP` text,
  `CMINVCNT` int(11) DEFAULT NULL,
  `CMPAYDAY` int(11) DEFAULT NULL,
  `CMNYRSALE` decimal(15,2) DEFAULT NULL,
  `CMNYRINCNT` int(11) DEFAULT NULL,
  `CMHOLDSW` text,
  `CMT1DESC` text,
  `CMT2DESC` text,
  `CMDISCTYPE` text,
  `CMSTARTDAT` text,
  `CMRENEWDAT` text,
  `CMPAYDAT` text,
  `CMINVDAT` text,
  `CMACTDAT` text,
  `CMMODDAT` text,
  `CMPHONE2` text,
  `CMTELEXT2` text,
  `CMTERR` text,
  `CMBUY1ST` text,
  `CMBUYLAST` text,
  `CMCUGROUP` text,
  `CMRPGROUP` text,
  `CMEMAIL` text,
  `CMPHONE3` text,
  `CMPHONE4` text,
  `CMTELEXT3` text,
  `CMTELEXT4` text,
  `CMTELDESC2` text,
  `CMTELDESC3` text,
  `CMTELDESC4` text,
  `CMDESRENEW` text,
  `CMDESDATE1` text,
  `CMDESDATE2` text,
  `CMDATE1` text,
  `CMDATE2` text,
  `CMBITMAP` text,
  `CMHANDLER` text,
  `CMPREFERHN` text,
  `CMRECALL` text,
  `CMRECDATE` text,
  `CMRECREM` text,
  `CMCURR` text,
  `CMFOLDER` text,
  `CMLYGROUP` text,
  `CMOTAXAPP` text,
  `CMOTAXCODE` text,
  `CMDISCRATE` decimal(15,2) DEFAULT NULL,
  `CMARPURGE` text,
  `CMLOCATION` text
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `SFCUMAST`
--

CREATE TABLE IF NOT EXISTS `SFCUMAST` (
  `CMCUCODE` text,
  `CMCUNAME` text,
  `CMADD1` text,
  `CMADD2` text,
  `CMPOST` text,
  `CMPHONE` text,
  `CMFAX1` text,
  `CMORDERAMT` decimal(15,2) DEFAULT NULL,
  `CMAVGPAY` decimal(15,1) DEFAULT NULL,
  `CMCREDLIMT` int(11) DEFAULT NULL,
  `CMYTDSALES` decimal(15,2) DEFAULT NULL,
  `CMYTDINCNT` int(11) DEFAULT NULL,
  `CMLYRSALE` decimal(15,2) DEFAULT NULL,
  `CMLYRINCNT` int(11) DEFAULT NULL,
  `CMSTATSW` text,
  `CMINTERSW` text,
  `CMBALANCE` decimal(15,2) DEFAULT NULL,
  `CMACCODE` text,
  `CMDEPT` text,
  `CMDISCOUNT` decimal(15,2) DEFAULT NULL,
  `CMDISCDAY` int(11) DEFAULT NULL,
  `CMNETDAY` int(11) DEFAULT NULL,
  `CMTITLE` text,
  `CMNAME1ST` text,
  `CMNAMESUR` text,
  `CMTELEXT1` text,
  `CMCITY` text,
  `CMSTATE` text,
  `CMCOUNTRY` text,
  `CMMEMO1` text,
  `CMFOB` text,
  `CMVIA` text,
  `CMSALESMAN` text,
  `CMTAXNUM1` text,
  `CMTAXNUM2` text,
  `CMTERMDESC` text,
  `CMGLGROUP` text,
  `CMINVCNT` int(11) DEFAULT NULL,
  `CMPAYDAY` int(11) DEFAULT NULL,
  `CMNYRSALE` decimal(15,2) DEFAULT NULL,
  `CMNYRINCNT` int(11) DEFAULT NULL,
  `CMHOLDSW` text,
  `CMT1DESC` text,
  `CMT2DESC` text,
  `CMDISCTYPE` text,
  `CMSTARTDAT` text,
  `CMRENEWDAT` text,
  `CMPAYDAT` text,
  `CMINVDAT` text,
  `CMACTDAT` text,
  `CMMODDAT` text,
  `CMPHONE2` text,
  `CMTELEXT2` text,
  `CMTERR` text,
  `CMBUY1ST` text,
  `CMBUYLAST` text,
  `CMCUGROUP` text,
  `CMRPGROUP` text,
  `CMEMAIL` text,
  `CMPHONE3` text,
  `CMPHONE4` text,
  `CMTELEXT3` text,
  `CMTELEXT4` text,
  `CMTELDESC2` text,
  `CMTELDESC3` text,
  `CMTELDESC4` text,
  `CMDESRENEW` text,
  `CMDESDATE1` text,
  `CMDESDATE2` text,
  `CMDATE1` text,
  `CMDATE2` text,
  `CMBITMAP` text,
  `CMHANDLER` text,
  `CMPREFERHN` text,
  `CMRECALL` text,
  `CMRECDATE` text,
  `CMRECREM` text,
  `CMCURR` text,
  `CMFOLDER` text,
  `CMOTAXAPP` text,
  `CMOTAXCODE` text,
  `CMCUSFIELD` text,
  `CMDISCRATE` decimal(15,2) DEFAULT NULL,
  `CMARPURGE` text,
  `CMLOCATION` text,
  `CMSHIPTO` text,
  `CMALSHIPTO` text,
  `CMMTAXCODE` text,
  `CMMTAXAPP` text
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

