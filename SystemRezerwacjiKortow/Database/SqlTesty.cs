using System;
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

            // testy postu
            //testInsertPost();
            //testUpdatePost();
            //SqlPost.GetPosts();
            //SqlPost.GetPost(1);
            //SqlPost.DeletePost(2);

            // testy rezerwacji
            //SqlReservation.GetReservationStateCourt(2, DateTime.Now, 0);
            //SqlReservation.SetReservationCourt(1, DateTime.Now.AddMinutes(5), DateTime.Now.AddHours(2), 4);
            //SqlReservation.GetReservations(4);
            //SqlReservation.AcceptReservation(1, false);
            //SqlReservation.CancelReservation(7, 4);
            //SqlReservation.GetReservation(7);
            //DateTime date;
            //date = new DateTime(2019, 3, 11, 16, 49, 0);
            //SqlReservation.GetReservationIDCourt(2, DateTime.Now.AddDays(2));
            //SqlReservation.GetReservationIDCourt(2, date);

            // testy rezerwacji cyklicznych
            //DateTime dateStart = new DateTime(2019, 3, 12, 16, 49, 0);
            //DateTime dateStop = new DateTime(2019, 5, 12, 16, 49, 0);
            //SqlCyclicReservation.SetReservationCourtCyclic(1, "Rezerwacja tygodniowa kort 1", dateStart, dateStop, 4, 2, 0, 0);
            //SqlCyclicReservation.GetReservationsCyclic(13);
            //SqlCyclicReservation.CancelReservationCyclic(9, 13);
            //SqlCyclicReservation.GetReservationCyclic(9);
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

        #region Post
        private static void testInsertPost()
        {
            Post post = new Post();
            post.TitlePL = "T_pl 3";
            post.TitleEN = "T_en 3";
            post.TitleDE = "T_de 3";
            post.DescriptionPL = "D_pl 3";
            post.DescriptionEN = "D_en 3";
            post.DescriptionDE = "D_de 3";
            SqlPost.InsertPost(post);
        }

        private static void testUpdatePost()
        {
            Post post = new Post();
            post.TitlePL = "111T_pl";
            post.TitleEN = "111T_en";
            post.TitleDE = "2222T_de";
            post.DescriptionPL = "2222D_pl";
            post.DescriptionEN = "111D_en";
            post.DescriptionDE = "222D_de";
            SqlPost.UpdatePost(2, post);
        }

        #endregion

    }
}