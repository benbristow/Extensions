namespace BenBristow.Extensions.UnitTests;

public sealed class StringExtensionsTests
{
    [Theory]
    [InlineData("hello", "Hello")]
    [InlineData("Hello", "Hello")]
    [InlineData("hELLO", "HELLO")]
    public void UppercaseFirstLetter_GivenString_ShouldConvertFirstLetterToUppercase(string input, string expected)
    {
        // Act
        var result = input.UppercaseFirstLetter();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("", "cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e")]
    [InlineData("test", "ee26b0dd4af7e749aa1a8ee3c10ae9923f618980772e473f8819a5d4940e0db27ac185f8a0e1d5f84f88bc887fd67b143732c304cc5fa9ad8e6f57f50028a8ff")]
    [InlineData("1234567890", "12b03226a6d8be9c6e8cd5e55dc6c7920caaa39df14aab92d5e3ea9340d1c8a4d3d0b8e4314f1f6ef131ba4bf1ceb9186ab87c801af0d5c95b1befb8cedae2b9")]
    public void HashSHA512_GivenString_ShouldReturnExpectedHash(string input, string expected)
    {
        // Act
        var result = input.HashSha512();

        // Assert
        result.Should().Be(expected);
    }
}