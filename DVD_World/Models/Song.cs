    using System.ComponentModel.DataAnnotations;

    namespace DVD_World.Models
    {
        public class Song
        {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Song Name is required.")]
        [MaxLength(200)]
        public string? SongName { get; set; }

        [Required(ErrorMessage = "Album ID is required.")]
        public int AlbumId { get; set; }

        public Album? Album { get; set; }

        [Required(ErrorMessage = "Artist ID is required.")]
        public int ArtistId { get; set; }

        public Artist? Artist { get; set; }

        [Required(ErrorMessage = "Genre is required.")]
        public GenreEnum? Genre { get; set; }

        [Required(ErrorMessage = "Year is required.")]
        public YearEnum? Year { get; set; }

        //[Required(ErrorMessage = "Song URL is required.")]
        //[Url(ErrorMessage = "Invalid URL format.")]
        public string? SongUrl { get; set; }

        //[Required(ErrorMessage = "Song image is required.")]
        //[Url(ErrorMessage = "Invalid URL format.")]
        public string? SongImage { get; set; }


        public enum GenreEnum
            {
                Rock,
                Pop,
                Jazz,
                Blues,
                Country,
                HipHop,
                Rap,
                NewAge,
                Gothic,
                IndustrialMetal,
                DeathMetal,
                BlackMetal,
                Emo,
                Screamo,
                Post
            }

            public enum YearEnum
            {
                Y2000 = 2000,
                Y2001 = 2001,
                Y2002 = 2002,
                Y2003 = 2003,
                Y2004 = 2004,
                Y2005 = 2005,
                Y2006 = 2006,
                Y2007 = 2007,
                Y2008 = 2008,
                Y2009 = 2009,
                Y2010 = 2010,
                Y2011 = 2011,
                Y2012 = 2012,
                Y2013 = 2013,
                Y2014 = 2014,
                Y2015 = 2015,
                Y2016 = 2016,
                Y2017 = 2017,
                Y2018 = 2018,
                Y2019 = 2019,
                Y2020 = 2020,
                Y2021 = 2021,
                Y2022 = 2022,
                Y2023 = 2023,
                Y2024 = 2024,
                Y2025 = 2025
            }
        }
    }
