using System;
using ExpressionTrees.Task2.ExpressionMapping.Converters;
using ExpressionTrees.Task2.ExpressionMapping.Tests.Models;
using Xunit;

namespace ExpressionTrees.Task2.ExpressionMapping.Tests
{
    internal class FullNameConverter : ITypeConverter<Foo, string>
    {
        public string From(Foo source)
        {
            return $"{source.FirstName} {source.LastName}";
        }
    }

    public class ExpressionMappingTests
    {
        [Fact]
        public void FromFooToBar()
        {
            var mapper = new MapperConfiguration<Foo, Bar>()
                .ForMember(source => source.Id, dest => dest.IdBar)
                .ForMember(source => source.Name, dest => dest.NameBar)
                .ForMember(
                    dest => dest.Number,
                    source => source.Number,
                    new IntToString())
                .ForMember(
                    dest => dest.FullName,
                    source => source,
                    new FullNameConverter())
                .Build();

            var foo = new Foo()
            {
                IdBar = 1,
                NameBar = "Name",
                Number = 12345,
                Offset = 101,
                FirstName = "FullName +",
                LastName = "LastName"
            };

            var res = mapper.Map(foo);

            Assert.Equal(foo.IdBar, res.Id);
            Assert.Equal(foo.NameBar, res.Name);
            Assert.Equal(foo.Number.ToString(), res.Number);
            Assert.Equal(foo.Offset, res.Offset);
            Assert.Equal("FullName + LastName", res.FullName);
        }
        
        [Fact]
        public void FromBarToFoo()
        {
            var mapper = new MapperConfiguration<Bar, Foo>()
                .ForMember(dest => dest.IdBar, source => source.Id)
                .ForMember(dest => dest.NameBar, source => source.Name)
                .ForMember(
                    dest => dest.Number,
                    source => source.Number,
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

        [Fact]
        public void WhenTwoRulesForOneProperty_ThrowAnError()
        {
            var mapper = new MapperConfiguration<Bar, Foo>();

            Assert.Throws<InvalidOperationException>(() =>
            {
                mapper
                    .ForMember(dest => dest.IdBar, source => source.Id)
                    .ForMember(dest => dest.IdBar, source => source.Id);
            });
        }
    }
}
