﻿Partial Public Class MathElement

    Public Overrides Function ToString() As String
        Return Export.ToString()
    End Function

    Public Overridable Function GetElementFromRelativePoint(Location As Point, Optional ConsiderOnlyChildren As Boolean = False, Optional ShouldBeIncluded As Boolean = False) As MathElement

        '
        ' Initialize variables for algorithm
        '
        Dim BestElement As MathElement = Nothing
        Dim BestIsIncluded As Boolean = False
        Dim BestHDistance As Double = Double.MaxValue
        Dim BestVDistance As Double = Double.MaxValue

        '
        ' Loop through every elements for best match
        '
        For Each Element In Children

            ' Gather information
            Dim CurrentIsIncluded As Boolean = Element.Export.LocationInParent.Contains(Location)
            Dim CurrentHDistance As Double = Math.Abs(Location.X - (Element.Export.LocationInParent.Left + Element.Export.LocationInParent.Right) / 2)
            Dim CurrentVDistance As Double = Math.Abs(Location.Y - (Element.Export.LocationInParent.Top + Element.Export.LocationInParent.Bottom) / 2)

            ' Check IsIncluded
            If Not BestIsIncluded And CurrentIsIncluded Then GoTo NewBestFound
            If BestIsIncluded And Not CurrentIsIncluded Then Continue For

            ' Check horizontal distance
            If CurrentHDistance < BestHDistance Then GoTo NewBestFound
            If CurrentHDistance > BestHDistance Then Continue For

            ' Check vertical distance
            If CurrentVDistance > BestVDistance Then Continue For

NewBestFound:
            ' Replace best element by current element
            BestElement = Element
            BestIsIncluded = CurrentIsIncluded
            BestHDistance = CurrentHDistance
            BestVDistance = CurrentVDistance

        Next

        '
        ' Check post conditions and return result
        '
        If Not ConsiderOnlyChildren AndAlso BestIsIncluded Then Return BestElement.GetElementFromRelativePoint(BestElement.ConvertPoint(Location, Me), ConsiderOnlyChildren, ShouldBeIncluded)
        If BestElement Is Nothing Then BestElement = Me : BestIsIncluded = True
        If ShouldBeIncluded AndAlso Not BestIsIncluded Then BestElement = Me : BestIsIncluded = True

        Return BestElement

    End Function

    Public Function GetSelectionPointFromRelativePoint(Location As Point) As SelectionHelper.SelectionPoint

        '
        ' Get the best element
        '
        Dim BestElement = GetElementFromRelativePoint(Location)

        '
        ' Create selection point from element
        '
        Dim TrueHDistance As Double = Location.X - (BestElement.Export.LocationIn(Me).Left + BestElement.Export.LocationIn(Me).Right) / 2
        If TrueHDistance >= 0 Then
            If BestElement.ParentElement.IsLayoutEngine Then
                Return BestElement.GetSelectionAfter()
            Else
                Return BestElement.GetSelectionAtEnd()
            End If
        Else
            If BestElement.ParentElement.IsLayoutEngine Then
                Return BestElement.GetSelectionBefore()
            Else
                Return BestElement.GetSelectionAtOrigin()
            End If
        End If

    End Function

    ''' <summary>
    ''' Convert a point relative to another element to a point relative to this element.
    ''' </summary>
    ''' <param name="Location">Original location to convert.</param>
    ''' <param name="RelativeTo">Element this location is relative to.</param>
    Public Function ConvertPoint(Location As Point, RelativeTo As MathElement) As Point
        Return ConvertRect(New Rect(Location, Location), RelativeTo).TopLeft
    End Function

    Public Function ConvertRect(Location As Rect, RelativeTo As MathElement) As Rect
        ' TODO: Check if this method is correct
        Dim CA = RelativeTo.GetCommonAncestrorWith(Me)
        Dim IntermediaryResult = FitRect(Location, RelativeTo.Export.SizeRect, RelativeTo.Export.LocationIn(CA))
        Return FitRect(IntermediaryResult, Me.Export.LocationIn(CA), Me.Export.SizeRect)
    End Function

    '++
    '++ Layout events
    '++

    Public Event SizeChanged As EventHandler
    Public Event VisualChanged As EventHandler

    '++
    '++ Default Fonts
    '++
    Public Shared ReadOnly DefaultLineHeight As Double = 1
    Public Shared ReadOnly DefaultFontSize As Double = 12 * 96 / 72
    Public Shared ReadOnly DefaultFonts As New Dictionary(Of FontTypes, List(Of FontFamily)) From {
        {
            FontTypes.SansSerif, New List(Of FontFamily) From {
                New FontFamily("Candara"), New FontFamily("Segoe UI"), New FontFamily("Verdana"), New FontFamily("Arial"), New FontFamily("Arial Unicode MS"), New FontFamily("Cambria Math"), New FontFamily("Cambria"), New FontFamily("STIXGeneral")
            }
        },
        {
            FontTypes.Serif, New List(Of FontFamily) From {
                New FontFamily("Cambria Math"), New FontFamily("Cambria"), New FontFamily("STIXGeneral")
            }
        },
        {
            FontTypes.DoubleStruck, New List(Of FontFamily) From {
                New FontFamily("Colonna MT"), New FontFamily("Cambria Math"), New FontFamily("Cambria"), New FontFamily("STIXGeneral")
            }
        },
        {
            FontTypes.Fraktur, New List(Of FontFamily) From {
                New FontFamily("Old English Text"), New FontFamily("Cambria Math"), New FontFamily("Cambria"), New FontFamily("STIXGeneral")
            }
        },
        {
            FontTypes.Monospace, New List(Of FontFamily) From {
                New FontFamily("Consolas"), New FontFamily("Courier New"), New FontFamily("Courier"), New FontFamily("Cambria Math"), New FontFamily("Cambria"), New FontFamily("STIXGeneral")
            }
        },
        {
            FontTypes.Script, New List(Of FontFamily) From {
                New FontFamily("MV Boli"), New FontFamily("Segoe Print"), New FontFamily("Lindsey"), New FontFamily("Cambria Math"), New FontFamily("Cambria"), New FontFamily("STIXGeneral")
            }
        }
    }

    '++
    '++ Font properties
    '++

    Public Sub ClearFontCache() Handles Me.Changed

        _Font = Nothing : For Each Child In Me.Children
            Child.ClearFontCache()
        Next

    End Sub

    Private _Font As Typeface
    Public Property Font As Typeface
        Get

            If _Font Is Nothing Then _Font = New Typeface(FontFamily, FontStyle, FontWeight, FontStretches.Normal)
            Return _Font

        End Get
        Set(value As Typeface)

            ' Setting a font is not possible, we are setting the sub properties instead
            ' It is probable that the result font will be different than the set value in some cases.

            FontFamily = value.FontFamily
            FontWeight = value.Weight
            FontStyle = value.Style
            ClearFontCache()

        End Set
    End Property

    Public Enum FontTypes
        SansSerif
        Serif
        Fraktur
        DoubleStruck
        Script
        Monospace
    End Enum

    Public Overridable ReadOnly Property FontType() As FontTypes
        Get
            Dim El As MathElement = Me, Att As String = Nothing

            Do
                If El.TryGetAttribute("mathvariant", Att) Then
                    If Att.Contains("monospace") Then Return FontTypes.Monospace
                    If Att.Contains("sans-serif") Then Return FontTypes.SansSerif
                    If Att.Contains("fraktur") Then Return FontTypes.Fraktur
                    If Att.Contains("script") Then Return FontTypes.Script
                    If Att.Contains("serif") Then Return FontTypes.Serif
                End If

                If El.TryGetAttribute("fontfamily", Att) Then
                    If Att.Contains("monospace") Then Return FontTypes.Monospace
                    If Att.Contains("sans-serif") Then Return FontTypes.SansSerif
                    If Att.Contains("fraktur") Then Return FontTypes.Fraktur
                    If Att.Contains("script") Then Return FontTypes.Script
                    If Att.Contains("serif") Then Return FontTypes.Serif
                End If

                If El.TryGetDefaultAttribute("mathvariant", Att) Then
                    If Att.Contains("monospace") Then Return FontTypes.Monospace
                    If Att.Contains("sans-serif") Then Return FontTypes.SansSerif
                    If Att.Contains("fraktur") Then Return FontTypes.Fraktur
                    If Att.Contains("script") Then Return FontTypes.Script
                    If Att.Contains("serif") Then Return FontTypes.Serif
                End If

                If El.TryGetDefaultAttribute("fontfamily", Att) Then
                    If Att.Contains("monospace") Then Return FontTypes.Monospace
                    If Att.Contains("sans-serif") Then Return FontTypes.SansSerif
                    If Att.Contains("fraktur") Then Return FontTypes.Fraktur
                    If Att.Contains("script") Then Return FontTypes.Script
                    If Att.Contains("serif") Then Return FontTypes.Serif
                End If

                El = El.ParentElement
            Loop Until El Is Nothing


            Return FontTypes.Serif
        End Get
    End Property

    Public Property FontFamily As FontFamily
        Get

            Dim Result As Object = Nothing
            If TryGetInheritedProperty("fontfamily", Parsers.ForFontFamily, Result) Then
                Return Result
            End If

            Return DefaultFonts(FontType).First()

        End Get
        Set(value As FontFamily)

            If value Is Nothing Then
                RemoveAttribute("fontfamily")
            Else
                SetAttribute("fontfamily", value.ToString())
            End If

        End Set
    End Property

    Public Sub SetMathVariant(value As String)
        If value Is Nothing Then
            RemoveAttribute("mathvariant")
        Else
            SetAttribute("mathvariant", value)
        End If
    End Sub

    Public Property FontStyle As FontStyle?
        Get

            Dim Result As Object = Nothing
            Dim El As MathElement = Me

            While El IsNot Nothing

                If TryGetProperty("fontstyle", Parsers.ForFontStyle, Result) Then
                    Return Result
                End If

                If TryGetAttribute("mathvariant", Result) Then
                    If Result.ToString().Contains("italic") Then Return FontStyles.Italic
                    If Result.ToString().Contains("oblique") Then Return FontStyles.Oblique
                    If Result.ToString().Contains("normal") Then Return FontStyles.Normal
                End If

                El = El.ParentElement

            End While

            Return FontStyles.Normal

        End Get
        Set(value As FontStyle?)

            If value Is Nothing Then
                RemoveAttribute("fontstyle")
            Else
                SetProperty("fontstyle", Parsers.ForFontStyle, value)
            End If

        End Set
    End Property

    Public Property FontWeight As FontWeight?
        Get

            Dim Result As Object = Nothing
            Dim El As MathElement = Me

            While El IsNot Nothing

                If TryGetProperty("fontweight", Parsers.ForFontWeight, Result) Then
                    Return Result
                End If

                If TryGetAttribute("mathvariant", Result) Then
                    If Result.ToString().Contains("bold") Then Return FontWeights.Bold
                    If Result.ToString().Contains("normal") Then Return FontWeights.Normal
                End If

                El = El.ParentElement

            End While

            Return FontWeights.Normal

            Return FontWeights.Normal
        End Get
        Set(value As FontWeight?)

            If value Is Nothing Then
                RemoveAttribute("fontweight")
            Else
                SetProperty("fontweight", Parsers.ForFontWeight, value)
            End If

        End Set
    End Property

    Public Property Foreground As Color?
        Get

            Dim Result As Object = Nothing
            If TryGetInheritedProperty(New String() {"color", "mathcolor"}, Parsers.ForColor, Result) Then
                Return Result
            End If

        End Get
        Set(value As Color?)

            If value Is Nothing Then
                RemoveAttribute("color") : RemoveAttribute("mathcolor")
            Else
                RemoveAttribute("color") : SetProperty("mathcolor", Parsers.ForColor, value)
            End If

        End Set
    End Property

    Public Property FontSize As Double?
        Get
            Dim Len As Length = Nothing
            If TryGetInheritedProperty("fontsize", Parsers.ForLength, Len) Then
                If ParentElement IsNot Nothing Then
                    Return Len.ToPixels(ParentElement.FontSize, ParentElement.FontSize)
                Else
                    Return Len.ToPixels(MathElement.DefaultFontSize, MathElement.DefaultFontSize)
                End If
            End If
                Return DefaultFontSize
        End Get
        Set(value As Double?)
            If value Is Nothing Then
                RemoveAttribute("fontsize")
            Else
                SetAttribute("fontsize", value & "px")
            End If
        End Set
    End Property

    Public Property Background As Color?
        Get
            ' TODO : Implement background
        End Get
        Set(value As Color?)

        End Set
    End Property

End Class
