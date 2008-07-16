using System;
using System.Collections;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;
using NetTopologySuite.Coordinates;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;
using SharpMap.Presentation;
using SharpMap.Presentation.Presenters;
using SharpMap.Presentation.Views;

namespace SharpMap.Tests.Presentation
{
    [TestFixture]
    public class ToolsPresenterTests
    {
        private IGeometryFactory _geoFactory;

        [TestFixtureSetUp]
        public void Setup()
        {
            BufferedCoordinate2DSequenceFactory sequenceFactory = new BufferedCoordinate2DSequenceFactory();
            _geoFactory = new GeometryFactory<BufferedCoordinate2D>(sequenceFactory);
        }

        [Test]
        public void PresenterSetsTools()
        {
            MockRepository mocks = new MockRepository();

            IToolsView view = mocks.Stub<IToolsView>();

            mocks.ReplayAll();

            Map map = new Map(_geoFactory);
            ToolsPresenter toolsPresenter = new ToolsPresenter(map, view);

            Assert.IsNotNull(view.Tools);
            Assert.AreNotEqual(0, view.Tools.Count);
        }

        //[Test]
        //public void ViewInitiatesToolSelectionChangeRequest()
        //{
        //    MockRepository mocks = new MockRepository();

        //    IToolsView view = mocks.Stub<IToolsView>();

        //    view.ToolChangeRequested += null;
        //    IEventRaiser toolChangeRequest = LastCall.IgnoreArguments().GetEventRaiser();

        //    mocks.ReplayAll();

        //    Map map = new Map(_geoFactory);
        //    ToolsPresenter toolsPresenter = new ToolsPresenter(map, view);

        //    Random rnd = new Random();
        //    Int32 toolIndex = rnd.Next(0, view.Tools.Count - 1);

        //    ToolChangeRequestedEventArgs requestArgs = new ToolChangeRequestedEventArgs(view.Tools[toolIndex]);
        //    toolChangeRequest.Raise(view, requestArgs);

        //    Assert.AreSame(view.SelectedTool, view.Tools[toolIndex]);
        //}
    }
}
