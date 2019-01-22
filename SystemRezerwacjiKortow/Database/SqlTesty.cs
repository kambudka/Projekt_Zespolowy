﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SystemRezerwacjiKortow.Models;

namespace SystemRezerwacjiKortow.Database
{
    public static class SqlTesty
    {
        public static void Testy()
        {
            // testy usera
            //testInsertUser();
            //testAddAddress();
            //SqlUser.CheckUserExists("alamakota@onet.pl");
            //SqlUser.CheckUserExists("admin@wp.pl");
            //testCheckEmailVeryfied();

            // testy kortu
            //testAddModyfyCourt();

            // testy reklamy
            //testGetAdvertisement();
            //testAddModyfyAdvertisement();
            //testDeleteAdvertisement();
            //SqlAdvertisement.GetAdvertisements(13);

        }
        #region User
        private static void testInsertUser()
        {
            User user = new User();
            user.FirstName = "Test15";
            user.Surname = "Test15";
            user.Email = "Test15";
            user.DateOfBirth = DateTime.Now;
            user.Password = "******";
            user.CustomerID = SqlDatabase.CustomerAtr;
            user.RoleID = SqlDatabase.UserRoleId;
            user.ActivationCode = Guid.NewGuid().ToString();

            SqlUser.InsertUser(user);
        }

        private static void testAddAddress()
        {
            User user = new User();
            user.UserID = 8;
            user.Email = "";
            Customer customer = new Customer();
            customer.CompanyName = "Test";
            customer.City = "Test";
            customer.Street = "Test";
            customer.ZipCode = "00-000";
            customer.DiscountValue = 0;

            SqlUser.AddModyfyAddress(customer, user.Email);
        }

        private static void testCheckEmailVeryfied()
        {
            UserLogin userLogin = new UserLogin();
            userLogin.Email = "admin@wp.pl";

            SqlUser.CheckEmailVeryfied(userLogin);
        }
        #endregion

        #region Court
        private static void testAddModyfyCourt()
        {
            Court court = new Court();
            court.CourtID = 0;

            court.CourtNumber = 2;
            court.SurfaceType = "ceglasty";
            court.IsForDoubles = true;
            court.IsCovered = false;
            court.PriceH = 99;
            court.Name = "kort 2";
            

            SqlCourt.AddModifyCourt(court);
        }
        #endregion

        #region Advertisement
        private static void testAddModyfyAdvertisement()
        {
            Advertisement advertisement = new Advertisement();
            advertisement.CourtID = 102;
            advertisement.DateFrom = DateTime.Now;
            advertisement.DateTo = DateTime.Now.AddDays(10);
            advertisement.UserID = 1;
            advertisement.Name = "Reklama 1";
            advertisement.Payment = 0;

            SqlAdvertisement.AddModifyAdvertisement(advertisement);
        }
        private static void testDeleteAdvertisement()
        {
            Advertisement advertisement = new Advertisement();
            List<Advertisement> list = SqlAdvertisement.GetAdvertisements(1);
            foreach(var item in list)
            {
                SqlAdvertisement.DeleteAdvertisement(item, 13);
            }
        }
        private static void testGetAdvertisement()
        {
            Advertisement advertisement = new Advertisement();
            List<Advertisement> list = SqlAdvertisement.GetAdvertisements(1);
            foreach (var item in list)
            {
                SqlAdvertisement.GetAdvertisement(item.DateFrom,item.DateTo, item.CourtID);
            }
        }
        #endregion

    }
}