using System;
using System.Collections;
using GeoAPI.Geometries;
using NetTopologySuite.Coordinates;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;
using SharpMap.Presentation;
using SharpMap.Presentation.Presenters;
using SharpMap.Presentation.Views;
using SharpMap.SimpleGeometries;

namespace SharpMap.Tests.Presentation
{
    [TestFixture]
    public class ToolsPresenterTests
    {
        private IGeometryFactory _geoFactory;

        [TestFixtureSetUp]
        public void Setup()
        {
            BufferedCoordinate2DFactory coordFactory = new BufferedCoordinate2DFactory();
            BufferedCoordinate2DSequenceFactory sequenceFactory = new BufferedCoordinate2DSequenceFactory();
            _geoFactory = new GeometryFactory(coordFactory, sequenceFactory);
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
            Assert.IsNotEmpty((ICollection) view.Tools);
        }

        [Test]
        public void ViewInitiatesToolSelectionChangeRequest()
        {
            MockRepository mocks = new MockRepository();

            IToolsView view = mocks.Stub<IToolsView>();

            view.ToolChangeRequested += null;
            IEventRaiser toolChangeRequest = LastCall.IgnoreArguments().GetEventRaiser();

            mocks.ReplayAll();

            Map map = new Map(_geoFactory);
            ToolsPresenter toolsPresenter = new ToolsPresenter(map, view);

            Random rnd = new Random();
            Int32 toolIndex = rnd.Next(0, view.Tools.Count - 1);

            ToolChangeRequestedEventArgs requestArgs = new ToolChangeRequestedEventArgs(view.Tools[toolIndex]);
            toolChangeRequest.Raise(view, requestArgs);

            Assert.AreSame(view.SelectedTool, view.Tools[toolIndex]);
        }
    }
}