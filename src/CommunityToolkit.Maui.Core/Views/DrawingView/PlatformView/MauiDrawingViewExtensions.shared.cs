using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Core.Views;
#if ANDROID
using Point = Android.Graphics.PointF;
# elif IOS || MACCATALYST
using Point = CoreGraphics.CGPoint;
#endif

namespace CommunityToolkit.Maui.Core.Extensions;

/// <summary>
/// Extension methods to support <see cref="IDrawingView"/>
/// </summary>
public static partial class MauiDrawingViewExtensions
{
	/// <summary>
	/// Get smoothed path.
	/// </summary>
	public static ObservableCollection<Point> SmoothedPathWithGranularity(this IEnumerable<Point> currentPoints, int granularity)
	{
		var currentPointsCopy = new List<Point>(currentPoints);

		// not enough points to smooth effectively, so return the original path and points.
		if (currentPointsCopy.Count < granularity + 2)
		{
			return new(currentPointsCopy);
		}

		var smoothedPoints = new List<Point>();

		// duplicate the first and last points as control points.
		currentPointsCopy.Insert(0, currentPointsCopy[0]);
		currentPointsCopy.Add(currentPointsCopy[^1]);

		// add the first point
		smoothedPoints.Add(currentPointsCopy[0]);

		var currentPointsCount = currentPointsCopy.Count;
		for (var index = 1; index < currentPointsCount - 2; index++)
		{
			var p0 = currentPointsCopy[index - 1];
			var p1 = currentPointsCopy[index];
			var p2 = currentPointsCopy[index + 1];
			var p3 = currentPointsCopy[index + 2];

			// add n points starting at p1 + dx/dy up until p2 using Catmull-Rom splines
			for (var i = 1; i < granularity; i++)
			{
				var t = i * (1f / granularity);
				var tt = t * t;
				var ttt = tt * t;

				// intermediate point
				var mid = GetIntermediatePoint(p0, p1, p2, p3, t, tt, ttt);
				smoothedPoints.Add(mid);
			}

			// add p2
			smoothedPoints.Add(p2);
		}

		// add the last point
		var last = currentPointsCopy[^1];
		smoothedPoints.Add(last);

		return new(smoothedPoints);
	}

	static Point GetIntermediatePoint(Point p0, Point p1, Point p2, Point p3, in float t, in float tt, in float ttt) => new()
	{
		X = 0.5f *
			((2f * p1.X) +
				((p2.X - p0.X) * t) +
				(((2f * p0.X) - (5f * p1.X) + (4f * p2.X) - p3.X) * tt) +
				(((3f * p1.X) - p0.X - (3f * p2.X) + p3.X) * ttt)),
		Y = 0.5f *
			((2 * p1.Y) +
				((p2.Y - p0.Y) * t) +
				(((2 * p0.Y) - (5 * p1.Y) + (4 * p2.Y) - p3.Y) * tt) +
				(((3 * p1.Y) - p0.Y - (3 * p2.Y) + p3.Y) * ttt))
	};
}