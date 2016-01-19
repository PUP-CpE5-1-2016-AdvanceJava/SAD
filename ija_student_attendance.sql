-- phpMyAdmin SQL Dump
-- version 4.2.11
-- http://www.phpmyadmin.net
--
-- Host: 127.0.0.1
-- Generation Time: Jan 19, 2016 at 04:19 AM
-- Server version: 5.6.21
-- PHP Version: 5.5.19

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Database: `ija_student_attendance`
--

-- --------------------------------------------------------

--
-- Table structure for table `attendance`
--

CREATE TABLE IF NOT EXISTS `attendance` (
`Id` int(11) NOT NULL,
  `StudentId` bigint(20) DEFAULT NULL,
  `Status` text,
  `Remarks` text,
  `Date` date DEFAULT NULL,
  `TimeIn` time DEFAULT NULL,
  `TimeOut` time DEFAULT NULL,
  `TimeSwipes` text
) ENGINE=InnoDB AUTO_INCREMENT=139 DEFAULT CHARSET=latin1;

--
-- Dumping data for table `attendance`
--

INSERT INTO `attendance` (`Id`, `StudentId`, `Status`, `Remarks`, `Date`, `TimeIn`, `TimeOut`, `TimeSwipes`) VALUES
(102, 1, 'Present', NULL, '2016-01-13', '09:10:01', NULL, '09:10:01.4401062,'),
(103, 2, 'Present', NULL, '2016-01-13', '09:10:02', NULL, '09:10:02.2438694,'),
(104, 3, 'Present', NULL, '2016-01-13', '09:10:03', NULL, '09:10:02.9833186,'),
(105, 51, 'Present', NULL, '2016-01-13', '09:10:08', NULL, '09:10:07.5661370,'),
(106, 52, 'Present', NULL, '2016-01-13', '09:10:08', NULL, '09:10:08.3088560,'),
(107, 53, 'Present', NULL, '2016-01-13', '09:10:09', NULL, '09:10:09.1120245,'),
(108, 14, 'Absent', NULL, '2016-01-13', NULL, NULL, NULL),
(109, 15, 'Absent', NULL, '2016-01-13', NULL, NULL, NULL),
(110, 16, 'Absent', NULL, '2016-01-13', NULL, NULL, NULL),
(111, 71, 'Absent', NULL, '2016-01-13', NULL, NULL, NULL),
(112, 72, 'Absent', NULL, '2016-01-13', NULL, NULL, NULL),
(113, 73, 'Absent', NULL, '2016-01-13', NULL, NULL, NULL),
(114, 123, 'Absent', NULL, '2016-01-13', NULL, NULL, NULL),
(115, 234, 'Absent', NULL, '2016-01-13', NULL, NULL, NULL),
(116, 345, 'Absent', NULL, '2016-01-13', NULL, NULL, NULL),
(117, 456, 'Absent', NULL, '2016-01-13', NULL, NULL, NULL),
(118, 31, 'Present', NULL, '2016-01-13', '11:32:42', '12:21:10', '11:32:41.5014914,12:21:08.0640609,12:21:09.0018812,12:21:09.5964478,'),
(119, 32, 'Present', NULL, '2016-01-13', '11:32:42', NULL, '11:32:42.4083854,'),
(120, 51, 'Present', NULL, '2016-01-14', '09:36:07', '09:42:34', '09:36:06.5379384,09:41:55.2899017,09:42:33.5383381,'),
(121, 1, 'Present', NULL, '2016-01-15', '19:04:19', '21:48:33', '19:04:18.7317249,19:10:13.4663075,19:12:14.5612571,19:35:06.3149619,21:17:35.6182127,21:18:00.1911359,21:22:07.5714361,21:23:25.4197093,21:31:59.4577860,21:46:29.1857257,21:47:57.2016599,21:48:33.0770487,'),
(122, 2, 'Present', NULL, '2016-01-15', '21:23:27', '21:48:02', '21:23:26.5892788,21:32:02.1636593,21:46:30.6580024,21:48:01.5450578,'),
(123, 3, 'Present', NULL, '2016-01-15', '21:23:28', NULL, '21:23:27.5615792,'),
(124, 51, 'Present', NULL, '2016-01-15', '21:32:10', NULL, '21:32:09.7814624,'),
(125, 53, 'Present', NULL, '2016-01-15', '21:46:32', '21:48:12', '21:46:31.9126370,21:48:11.7885103,'),
(126, 5680887, 'Present', NULL, '2016-01-15', '21:56:19', '22:33:07', '21:56:19.2637618,21:56:23.4408805,21:56:32.3524878,21:58:32.9885441,22:33:06.8069443,'),
(127, 5672926, 'Present', NULL, '2016-01-15', '21:56:21', '22:33:05', '21:56:21.4725813,21:56:25.2269702,21:56:29.9385139,21:58:35.0775683,22:33:04.7573485,'),
(128, 1, 'Present', NULL, '2016-01-16', '00:52:16', '23:30:03', '00:52:15.7229894,00:57:04.1617879,10:49:52.1024145,10:52:25.3648603,10:52:53.1473685,12:15:41.6968307,23:24:27.1083204,23:30:03.2466599,'),
(129, 2, 'Late', NULL, '2016-01-16', '10:49:56', '23:30:05', '10:49:56.4351074,10:52:54.1408780,23:24:28.7357351,23:30:05.1755941,'),
(130, 3, 'Late', NULL, '2016-01-16', '10:49:58', '10:54:01', '10:49:58.4069691,10:52:54.9459059,10:54:01.2561795,'),
(131, 51, 'Late', NULL, '2016-01-16', '10:50:06', '23:36:39', '10:50:05.5133898,10:54:02.2956397,23:32:43.2891517,23:36:16.3225719,23:36:39.4541520,'),
(132, 52, 'Present', NULL, '2016-01-16', '10:54:03', '23:36:18', '10:54:03.4118681,23:32:47.2877806,23:36:18.2839890,'),
(133, 53, 'Present', NULL, '2016-01-16', '10:54:05', '23:36:26', '10:54:04.6342785,23:36:25.7347178,'),
(138, 5672926, 'Late', NULL, '2016-01-19', '11:16:42', NULL, '11:16:41.8015216,');

-- --------------------------------------------------------

--
-- Table structure for table `settings`
--

CREATE TABLE IF NOT EXISTS `settings` (
`Id` int(11) NOT NULL,
  `classTime` time NOT NULL,
  `lateTime` time NOT NULL,
  `autoMailTime` time NOT NULL,
  `autoMailTo` varchar(150) NOT NULL,
  `autoMailCC` varchar(150) NOT NULL,
  `autoMailSubj` varchar(100) NOT NULL,
  `autoMailMsg` text NOT NULL,
  `autoMailUser` varchar(100) NOT NULL,
  `autoMailPass` text NOT NULL,
  `autoMailChkBox` tinyint(1) NOT NULL,
  `videoPathBox` varchar(300) NOT NULL
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=latin1;

--
-- Dumping data for table `settings`
--

INSERT INTO `settings` (`Id`, `classTime`, `lateTime`, `autoMailTime`, `autoMailTo`, `autoMailCC`, `autoMailSubj`, `autoMailMsg`, `autoMailUser`, `autoMailPass`, `autoMailChkBox`, `videoPathBox`) VALUES
(5, '07:00:00', '19:30:00', '20:00:00', 'surnet_rafael@yahoo.com', 'nolasco_arian@yahoo.com', 'Infant Jesus Academy automail attendance', 'Hoho', 'nolascoarian@gmail.com', 'ueKQXOtIk7IQ+K7LZndNmw==', 1, 'C:\\Users\\arar\\Videos\\Dance\\megajam_JasMeakin\\%Bang Bang% Jessie J - Ariana G - Nicki M choreography by Jasmine Meakin (Mega Jam).mp4');

-- --------------------------------------------------------

--
-- Table structure for table `students`
--

CREATE TABLE IF NOT EXISTS `students` (
`Id` bigint(20) NOT NULL,
  `Fname` varchar(50) NOT NULL,
  `Mname` varchar(50) NOT NULL,
  `Sname` varchar(50) NOT NULL,
  `Grade` int(11) NOT NULL,
  `Section` varchar(100) NOT NULL
) ENGINE=InnoDB AUTO_INCREMENT=123456790 DEFAULT CHARSET=latin1;

--
-- Dumping data for table `students`
--

INSERT INTO `students` (`Id`, `Fname`, `Mname`, `Sname`, `Grade`, `Section`) VALUES
(1, 'kkkkk', 'ppp', 'ooo', 5, 'haha'),
(2, 'popo', 'lklk', 'koko', 5, 'haha'),
(3, 'iiuiu', 'iuiu', 'jkjk', 5, 'haha'),
(14, 'qweqwesdf', 'asd', 'ssd', 5, 'aasd'),
(15, 'klklk', 'lklklk', 'lklk', 5, 'lklk'),
(16, 'new', 'new', 'new', 5, 'Makulet'),
(31, 'tintin', 'klkj', 'Man', 4, 'Zeus'),
(32, 'tintintin', 'asdqw', 'kjkj', 4, 'zeus'),
(33, 'christin', 'lkklkj', 'oipi', 4, 'zeus'),
(34, 'nitnit', 'poiqwe', 'qwe', 4, 'zeus'),
(51, 'qweqw', 'wqeqwe', 'aaa', 5, 'haha'),
(52, 'qweqasda', 'asdasd', 'bbb', 5, 'haha'),
(53, 'qwerh', 'ghgfh', 'ccc', 5, 'haha'),
(71, 'abc', 'abc', 'abc', 5, 'haha'),
(72, 'bcd', 'bcd', 'bcd', 5, 'haha'),
(73, 'cde', 'cde', 'cde', 5, 'haha'),
(123, 'Arian', 'Sario', 'Nolasco', 5, 'Datu Sumakwel'),
(234, 'Rafael John', 'Boltron', 'Surnet', 1, 'Juan Luna'),
(345, 'Ramil', 'Lucot', 'Villanueva', 3, 'Datu Urduja'),
(456, 'Lee Arvi', 'Banaag', 'Real', 3, 'Datu Puti'),
(5672926, 'Idcard', 'TestUd', 'VerzoId', 5, 'haha'),
(5680887, 'jojo', 'selso', 'mira', 5, 'hoho'),
(123456789, 'Rafael John', 'Boltron', 'Surnet', 5, 'Rizal');

-- --------------------------------------------------------

--
-- Table structure for table `users`
--

CREATE TABLE IF NOT EXISTS `users` (
`Id` int(11) NOT NULL,
  `Username` varchar(50) DEFAULT NULL,
  `Userpass` text,
  `Usertype` varchar(50) DEFAULT NULL
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=latin1;

--
-- Dumping data for table `users`
--

INSERT INTO `users` (`Id`, `Username`, `Userpass`, `Usertype`) VALUES
(2, 'arar', 'a39da23fd6d9da5da2abe407f2e8fa7e', 'Administrator'),
(4, 'jp', '55add3d845bfcd87a9b0949b0da49c0a', 'Administrator'),
(5, 'monitor', '08b5411f848a2581a41672a759c87380', 'Monitor'),
(6, 'obs', 'dbabb977cec6af84577d101d6053b0b1', 'Observer');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `attendance`
--
ALTER TABLE `attendance`
 ADD PRIMARY KEY (`Id`);

--
-- Indexes for table `settings`
--
ALTER TABLE `settings`
 ADD PRIMARY KEY (`Id`);

--
-- Indexes for table `students`
--
ALTER TABLE `students`
 ADD PRIMARY KEY (`Id`);

--
-- Indexes for table `users`
--
ALTER TABLE `users`
 ADD PRIMARY KEY (`Id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `attendance`
--
ALTER TABLE `attendance`
MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=139;
--
-- AUTO_INCREMENT for table `settings`
--
ALTER TABLE `settings`
MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=6;
--
-- AUTO_INCREMENT for table `students`
--
ALTER TABLE `students`
MODIFY `Id` bigint(20) NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=123456790;
--
-- AUTO_INCREMENT for table `users`
--
ALTER TABLE `users`
MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=7;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
