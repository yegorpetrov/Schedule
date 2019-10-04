using Schedule;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ScheduleTest
{
    public class ListExpressionTests
    {
        private int ListNext(string expression, int current)
        {
            new ListExpression(expression).Find(current, ref current, true);
            return current;
        }

        private int ListPrevious(string expression, int current)
        {
            new ListExpression(expression).Find(current, ref current, false);
            return current;
        }

        [Fact]
        public void SingleItemListExpressionReturnsThatItem()
        {
            Assert.Equal(0, ListNext("0", 0));
            Assert.Equal(int.MaxValue, ListNext(int.MaxValue.ToString(), 0));
        }

        [Fact]
        public void BasicListReturnsTheCurrentItem()
        {
            Assert.Equal(4, ListNext("4,5", 4));
            Assert.Equal(9, ListNext("4,5,6,7,8,9", 9));
        }

        [Fact]
        public void MultiRangeListWorksForward()
        {
            Assert.Equal(01, ListNext("1,2,3-5,10-20/3", 00));
            Assert.Equal(01, ListNext("1,2,3-5,10-20/3", 01));
            Assert.Equal(02, ListNext("1,2,3-5,10-20/3", 02));
            Assert.Equal(03, ListNext("1,2,3-5,10-20/3", 03));
            Assert.Equal(04, ListNext("1,2,3-5,10-20/3", 04));
            Assert.Equal(05, ListNext("1,2,3-5,10-20/3", 05));
            Assert.Equal(10, ListNext("1,2,3-5,10-20/3", 06));
            Assert.Equal(10, ListNext("1,2,3-5,10-20/3", 07));
            Assert.Equal(10, ListNext("1,2,3-5,10-20/3", 08));
            Assert.Equal(10, ListNext("1,2,3-5,10-20/3", 09));
            Assert.Equal(10, ListNext("1,2,3-5,10-20/3", 10));
            Assert.Equal(13, ListNext("1,2,3-5,10-20/3", 11));
            Assert.Equal(13, ListNext("1,2,3-5,10-20/3", 12));
            Assert.Equal(13, ListNext("1,2,3-5,10-20/3", 13));
            Assert.Equal(16, ListNext("1,2,3-5,10-20/3", 14));
            Assert.Equal(16, ListNext("1,2,3-5,10-20/3", 15));
            Assert.Equal(16, ListNext("1,2,3-5,10-20/3", 16));
            Assert.Equal(20, ListNext("1,2,3-5,10-20/3", 17));
            Assert.Equal(20, ListNext("1,2,3-5,10-20/3", 18));
            Assert.Equal(20, ListNext("1,2,3-5,10-20/3", 19));
            Assert.Equal(20, ListNext("1,2,3-5,10-20/3", 20));
            Assert.Equal(20, ListNext("1,2,3-5,10-20/3", 21));
        }

        [Fact]
        public void MultiRangeListWorksBackward()
        {
            Assert.Equal(20, ListPrevious("1,2,3-5,10-20/3", 21));
            Assert.Equal(20, ListPrevious("1,2,3-5,10-20/3", 20));
            Assert.Equal(20, ListPrevious("1,2,3-5,10-20/3", 19));
            Assert.Equal(16, ListPrevious("1,2,3-5,10-20/3", 18));
            Assert.Equal(16, ListPrevious("1,2,3-5,10-20/3", 17));
            Assert.Equal(16, ListPrevious("1,2,3-5,10-20/3", 16));
            Assert.Equal(13, ListPrevious("1,2,3-5,10-20/3", 15));
            Assert.Equal(13, ListPrevious("1,2,3-5,10-20/3", 14));
            Assert.Equal(13, ListPrevious("1,2,3-5,10-20/3", 13));
            Assert.Equal(10, ListPrevious("1,2,3-5,10-20/3", 12));
            Assert.Equal(10, ListPrevious("1,2,3-5,10-20/3", 11));
            Assert.Equal(10, ListPrevious("1,2,3-5,10-20/3", 10));
            Assert.Equal(05, ListPrevious("1,2,3-5,10-20/3", 09));
            Assert.Equal(05, ListPrevious("1,2,3-5,10-20/3", 08));
            Assert.Equal(05, ListPrevious("1,2,3-5,10-20/3", 07));
            Assert.Equal(05, ListPrevious("1,2,3-5,10-20/3", 06));
            Assert.Equal(05, ListPrevious("1,2,3-5,10-20/3", 05));
            Assert.Equal(04, ListPrevious("1,2,3-5,10-20/3", 04));
            Assert.Equal(03, ListPrevious("1,2,3-5,10-20/3", 03));
            Assert.Equal(02, ListPrevious("1,2,3-5,10-20/3", 02));
            Assert.Equal(01, ListPrevious("1,2,3-5,10-20/3", 01));
        }
    }
}
