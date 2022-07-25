using ExpressionTrees.Task2.ExpressionMapping.Converters;
using ExpressionTrees.Task2.ExpressionMapping.Tests.Models;
using Xunit;

namespace ExpressionTrees.Task2.ExpressionMapping.Tests
{
    public class ExpressionMappingTests
    {
        [Fact]
        public void FromFooToBar()
        {
            var mapper = new MapperConfiguration<Foo, Bar>()
                .ForMember(source => source.IdBar, dest => dest.Id)
                .ForMember(source => source.NameBar, dest => dest.Name)
                .ForMember(
                    source => source.Number,
                    dest => dest.Number,
                    new IntToString())
                .Build();

            var foo = new Foo()
            {
                IdBar = 1,
                NameBar = "Name",
                Number = 12345,
                Offset = 101,
            };

            var res = mapper.Map(foo);

            Assert.Equal(foo.IdBar, res.Id);
            Assert.Equal(foo.NameBar, res.Name);
            Assert.Equal(foo.Number.ToString(), res.Number);
            Assert.Equal(foo.Offset, res.Offset);
        }
        
        [Fact]
        public void FromBarToFoo()
        {
            var mapper = new MapperConfiguration<Bar, Foo>()
                .ForMember(source => source.Id, dest => dest.IdBar)
                .ForMember(source => source.Name, dest => dest.NameBar)
                .ForMember(
                    source => source.Number,
                    dest => dest.Number,
                    new StringToInt())
                .Build();

            var bar = new Bar()
            {
                Id = 1,
                Name = "Name",
                Number = "12345",
                Offset = 101,
            };

            var res = mapper.Map(bar);

            Assert.Equal(bar.Id, res.IdBar);
            Assert.Equal(bar.Name, res.NameBar );
            Assert.Equal(bar.Number, res.Number.ToString());
            Assert.Equal(bar.Offset, res.Offset);
        }
    }
}
