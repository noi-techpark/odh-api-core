using Xunit;
using Helper;
using System.Collections.Generic;

namespace OdhApiCoreTests.Helper
{
    public class PostgresSQLTransformerTests
    {
        public static TheoryData<string> GetLanguages()
        {
            return new TheoryData<string>
            {
                "de", "it", "en", "ru"
            };
        }

        [Theory]
        [MemberData(nameof(GetLanguages))]
        public void TransformAccommodationToMobileDataObject_EmptyAccomodation(string language)
        {
            var accomodation = new Accommodation();
            MobileData result = PostgresSQLTransformer.TransformAccommodationToMobileDataObject(
                accomodation, language);
            Assert.NotNull(result);
        }

        [Theory]
        [MemberData(nameof(GetLanguages))]
        public void TransformToAccommodationLocalized_EmptyAccomodation(string language)
        {
            var accomodation = new Accommodation();
            AccommodationLocalized result = PostgresSQLTransformer.TransformToAccommodationLocalized(
                accomodation, language);
            Assert.NotNull(result);
        }

        [Theory]
        [MemberData(nameof(GetLanguages))]
        public void TransformToAccommodationListObject_EmptyAccomodation(string language)
        {
            var accomodation = new Accommodation();
            AccoListObject result = PostgresSQLTransformer.TransformToAccommodationListObject(
                accomodation, language);
            Assert.NotNull(result);
        }

        [Theory]
        [MemberData(nameof(GetLanguages))]
        public void TransformToGBLTSActivityPoiLocalized_EmptyPoi(string language)
        {
            var poi = new GBLTSPoi();
            GBLTSActivityPoiLocalized result = PostgresSQLTransformer.TransformToGBLTSActivityPoiLocalized(
                poi, language);
            Assert.NotNull(result);
        }

        [Theory]
        [MemberData(nameof(GetLanguages))]
        public void TransformToGastronomyLocalized_EmptyGastronomy(string language)
        {
            var gastronomy = new Gastronomy();
            GastronomyLocalized result = PostgresSQLTransformer.TransformToGastronomyLocalized(
                gastronomy, language);
            Assert.NotNull(result);
        }

        [Theory]
        [MemberData(nameof(GetLanguages))]
        public void TransformToEventLocalized_EmptyEvent(string language)
        {
            var @event = new Event();
            EventLocalized result = PostgresSQLTransformer.TransformToEventLocalized(
                @event, language);
            Assert.NotNull(result);
        }

        [Theory]
        [MemberData(nameof(GetLanguages))]
        public void TransformToArticleBaseInfosLocalized_EmptyArticle(string language)
        {
            var article = new Article();
            ArticleBaseInfosLocalized result = PostgresSQLTransformer.TransformToArticleBaseInfosLocalized(
                article, language);
            Assert.NotNull(result);
        }

        [Theory]
        [MemberData(nameof(GetLanguages))]
        public void TransformToODHActivityPoiLocalized_EmptyODHActivityPoi(string language)
        {
            var activity = new ODHActivityPoi();
            ODHActivityPoiLocalized result = PostgresSQLTransformer.TransformToODHActivityPoiLocalized(
                activity, language);
            Assert.NotNull(result);
        }

        [Theory]
        [MemberData(nameof(GetLanguages))]
        public void TransformToPackageLocalized_EmptyPackage(string language)
        {
            var package = new Package();
            PackageLocalized result = PostgresSQLTransformer.TransformToPackageLocalized(
                package, language);
            Assert.NotNull(result);
        }

        [Theory]
        [MemberData(nameof(GetLanguages))]
        public void TransformToPackageBooklist_EmptyPackage(string language)
        {
            var package = new Package();
            PackageBookList result = PostgresSQLTransformer.TransformToPackageBooklist(
                package, language);
            Assert.NotNull(result);
        }
        
        [Theory]
        [MemberData(nameof(GetLanguages))]
        public void TransformToBaseInfosLocalized_EmptyBaseInfos(string language)
        {
            var region = new Region();
            BaseInfosLocalized result = PostgresSQLTransformer.TransformToBaseInfosLocalized(
                region, language);
            Assert.NotNull(result);
        }
    }
}
