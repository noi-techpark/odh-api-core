using Xunit;
using Helper;

namespace OdhApiCoreTests.Helper
{
    public class PostgresSQLTransformerTests
    {
        [Fact]
        public void TransformAccommodationToMobileDataObject_EmptyAccomodation()
        {
            var accomodation = new Accommodation();
            string language = "de";
            MobileData result = PostgresSQLTransformer.TransformAccommodationToMobileDataObject(
                accomodation, language);
            Assert.NotNull(result);
        }

        [Fact]
        public void TransformToAccommodationLocalized_EmptyAccomodation()
        {
            var accomodation = new Accommodation();
            string language = "de";
            AccommodationLocalized result = PostgresSQLTransformer.TransformToAccommodationLocalized(
                accomodation, language);
            Assert.NotNull(result);
        }

        [Fact]
        public void TransformToAccommodationListObject_EmptyAccomodation()
        {
            var accomodation = new Accommodation();
            string language = "de";
            AccoListObject result = PostgresSQLTransformer.TransformToAccommodationListObject(
                accomodation, language);
            Assert.NotNull(result);
        }

        [Fact]
        public void TransformToGBLTSActivityPoiLocalized_EmptyPoi()
        {
            var poi = new GBLTSPoi();
            string language = "de";
            GBLTSActivityPoiLocalized result = PostgresSQLTransformer.TransformToGBLTSActivityPoiLocalized(
                poi, language);
            Assert.NotNull(result);
        }

        [Fact]
        public void TransformToGastronomyLocalized_EmptyGastronomy()
        {
            var gastronomy = new Gastronomy();
            string language = "de";
            GastronomyLocalized result = PostgresSQLTransformer.TransformToGastronomyLocalized(
                gastronomy, language);
            Assert.NotNull(result);
        }

        [Fact]
        public void TransformToEventLocalized_EmptyEvent()
        {
            var @event = new Event();
            string language = "de";
            EventLocalized result = PostgresSQLTransformer.TransformToEventLocalized(
                @event, language);
            Assert.NotNull(result);
        }

        [Fact]
        public void TransformToArticleBaseInfosLocalized_EmptyArticle()
        {
            var article = new Article();
            string language = "de";
            ArticleBaseInfosLocalized result = PostgresSQLTransformer.TransformToArticleBaseInfosLocalized(
                article, language);
            Assert.NotNull(result);
        }

        [Fact]
        public void TransformToODHActivityPoiLocalized_EmptyODHActivityPoi()
        {
            var activity = new ODHActivityPoi();
            string language = "de";
            ODHActivityPoiLocalized result = PostgresSQLTransformer.TransformToODHActivityPoiLocalized(
                activity, language);
            Assert.NotNull(result);
        }

        [Fact]
        public void TransformToPackageLocalized_EmptyPackage()
        {
            var package = new Package();
            string language = "de";
            PackageLocalized result = PostgresSQLTransformer.TransformToPackageLocalized(
                package, language);
            Assert.NotNull(result);
        }

        [Fact]
        public void TransformToPackageBooklist_EmptyPackage()
        {
            var package = new Package();
            string language = "de";
            PackageBookList result = PostgresSQLTransformer.TransformToPackageBooklist(
                package, language);
            Assert.NotNull(result);
        }
        
        [Fact]
        public void TransformToBaseInfosLocalized_EmptyBaseInfos()
        {
            var region = new Region();
            string language = "de";
            BaseInfosLocalized result = PostgresSQLTransformer.TransformToBaseInfosLocalized(
                region, language);
            Assert.NotNull(result);
        }
    }
}
