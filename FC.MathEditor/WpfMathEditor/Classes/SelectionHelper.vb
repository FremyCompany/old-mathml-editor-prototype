'Partial Public Class SelectionHelper0
'    Implements IEnumerable(Of MathElement)

'    Protected This As MathDocument
'    Private WithEvents CA_SEL, CA_START, CA_END As MathElement

'    Public Sub New(ByVal This As MathDocument)
'        Me.This = This
'        SetSelection(This, This.LastChild, Nothing)
'    End Sub

'    Private IsSelectionChanging As Boolean
'    Public Event BeforeSelectionChanged As EventHandler
'    Public Event SelectionChanged As EventHandler

'    Private Function GetSelectedElements() As IEnumerable(Of MathElement)
'        If IsEmpty Then Return New MathElement() {}
'        Return New SiblingEnumeratorGenerator(CommonAncestror.Children.After(SelectionStart), CommonAncestror.Children.Before(SelectionEnd))
'    End Function

'    Private Function CloneSelectedElements() As IEnumerable(Of MathElement)
'        Return GetSelectedElements().Select(Function(el) el.Clone())
'    End Function

'    Public Sub DeleteContents()
'        For Each el In GetSelectedElements().ToArray()
'            CommonAncestror.RemoveChild(el)
'        Next
'    End Sub

'    Public Sub ReplaceContents(ByVal NewChilds As IEnumerable(Of MathElement))
'        Me.DeleteContents()
'        For Each el In NewChilds
'            CommonAncestror.Children.InsertBefore(el, SelectionEnd)
'        Next
'        SetSelection(CommonAncestror, CommonAncestror.Children.Before(SelectionEnd), SelectionEnd)
'    End Sub

'    Public Sub ReplaceContents(ByVal NewChild As MathElement)
'        Me.DeleteContents()
'        CommonAncestror.Children.InsertBefore(NewChild, SelectionEnd)
'        SetSelection(CommonAncestror, NewChild, SelectionEnd)
'    End Sub

'    Private Sub SetSelection(ByVal StartPoint As Selection, ByVal EndPoint As Selection)

'        ' Compute _Selection
'        Dim SS = StartPoint.SelectionStart
'        Dim SE = EndPoint.SelectionEnd

'        ' Collapse selection points
'        StartPoint = New Selection(StartPoint.CommonAncestror, SS, StartPoint.CommonAncestror.Children.After(SS))
'        EndPoint = New Selection(EndPoint.CommonAncestror, EndPoint.CommonAncestror.Children.After(SE), SE)

'        Dim CA = If(SS, SE)
'        If StartPoint.CommonAncestror Is EndPoint.CommonAncestror Then
'            CA = StartPoint.CommonAncestror
'        Else
'            CA = StartPoint.CommonAncestror.GetCommonAncestrorWith(EndPoint.CommonAncestror)

'            While SS.ParentElement IsNot CA
'                SS = SS.ParentElement
'            End While

'            While SE.ParentElement IsNot SE
'                SE = SE.ParentElement
'            End While

'        End If

'        ' Invert StartPoint and EndPoint if needed
'        Dim Dir As SelectionDirection = SelectionDirection.LTR
'        If (SE IsNot Nothing) AndAlso (SS IsNot Nothing) AndAlso (SE Is SS OrElse SE.IsBefore(SS)) Then
'            Dir = SelectionDirection.RTL
'            Dim ST = SE.PreviousSibling
'            SE = SS.NextSibling : SS = ST
'        End If

'        ' Set the selection
'        SetSelection_Internal(New Selection(CA, SS, SE), StartPoint, EndPoint)
'        _Dir = Dir

'    End Sub

'    Public Sub SetSelection(ByVal NewPoint As Selection, Optional ByVal PointToChange As SelectionPointType = SelectionPointType.Selection)
'        Select Case PointToChange
'            Case SelectionPointType.Selection
'                SetSelection(NewPoint.CommonAncestror, NewPoint.SelectionStart, NewPoint.SelectionEnd)
'            Case SelectionPointType.StartPoint
'                SetSelection(NewPoint, EndPoint)
'            Case SelectionPointType.EndPoint
'                SetSelection(StartPoint, NewPoint)
'            Case Else
'                Throw New ArgumentException("Unknown value for the selection point type", "PointToChange")
'        End Select
'    End Sub

'    Public Function GetSelection(ByVal PointToRetreive As SelectionPointType) As Selection
'        Select Case PointToRetreive
'            Case SelectionPointType.Selection
'                Return _Selection
'            Case SelectionPointType.StartPoint
'                Return _StartPoint
'            Case SelectionPointType.EndPoint
'                Return _EndPoint
'            Case Else
'                Throw New ArgumentException("Unknown value for the selection point type", "PointToRetreive")
'        End Select
'    End Function

'    Public Sub SetSelection(ByVal CommonAncestror As MathElement, ByVal SelectionStart As MathElement, ByVal SelectionEnd As MathElement)

'        ' Check if the selection is valid
'        If (
'            (CommonAncestror IsNot Nothing) _
'            AndAlso (CommonAncestror.ParentDocument Is This) _
'            AndAlso (SelectionStart Is Nothing OrElse SelectionStart.ParentElement Is CommonAncestror) _
'            AndAlso (SelectionEnd Is Nothing OrElse SelectionEnd.ParentElement Is CommonAncestror)
'        ) Then

'            ' Invert StartPoint and EndPoint if needed
'            Dim SS = SelectionStart, SE = SelectionEnd, CA = CommonAncestror
'            If (SS Is Nothing) OrElse (SE IsNot Nothing AndAlso (SE Is SS OrElse SE.IsBefore(SS))) Then
'                _Dir = SelectionDirection.RTL
'                Dim ST = If(SE Is Nothing, Nothing, SE.PreviousSibling)
'                SE = SS : SS = SE
'            Else
'                _Dir = SelectionDirection.LTR
'            End If

'            ' Perform the selection change
'            SetSelection_Internal(
'                New Selection(CA, SS, SE),
'                New Selection(CA, SS, CA.Children.After(SS)),
'                New Selection(CA, CA.Children.Before(SE), SE)
'            )

'        Else

'            ' Notify of an error
'            Throw New ArgumentException("Selection was invalid.")

'        End If

'    End Sub

'    Private Sub SetSelection_Internal(ByVal Selection As Selection, ByVal StartPoint As Selection, ByVal EndPoint As Selection)

'        ' Verify the direction of the selection
'        If Selection.SelectionEnd IsNot Nothing AndAlso Selection.SelectionEnd.IsBefore(SelectionStart) Then
'            Dim Temp = Selection.SelectionEnd
'            Selection.SelectionEnd = Selection.SelectionStart
'            Selection.SelectionStart = Temp
'            _Dir = SelectionDirection.RTL
'        Else
'            ' TODO: Some SetSelection perfor the _Dir modification themselves...
'            _Dir = SelectionDirection.LTR
'        End If

'        ' Raise the BeforeSelectionChanged event
'        If Not IsSelectionChanging Then
'            ' TODO: What about IsSelectionChanging and SelectionChanged ????
'            RaiseEvent BeforeSelectionChanged(Me, EventArgs.Empty)
'        End If

'        ' Perform the changes
'        _Selection = Selection
'        _StartPoint = StartPoint
'        _EndPoint = EndPoint

'        CA_START = StartPoint.CommonAncestror
'        CA_END = EndPoint.CommonAncestror
'        CA_SEL = Selection.CommonAncestror

'    End Sub

'    Public Sub ReverseSelection()
'        Dim Temp = _StartPoint
'        _StartPoint = _EndPoint
'        _EndPoint = Temp
'        _Dir = Not _Dir
'    End Sub

'    Private _Selection As Selection
'    Public ReadOnly Property CommonAncestror As MathElement
'        Get
'            Return _Selection.CommonAncestror
'        End Get
'    End Property

'    Public ReadOnly Property SelectionStart As MathElement
'        Get
'            Return _Selection.SelectionStart
'        End Get
'    End Property

'    Public ReadOnly Property SelectionEnd As MathElement
'        Get
'            Return _Selection.SelectionEnd
'        End Get
'    End Property

'    Private _StartPoint As Selection
'    Public ReadOnly Property StartPoint As Selection
'        Get
'            Return _StartPoint
'        End Get
'    End Property

'    Private _EndPoint As Selection
'    Public ReadOnly Property EndPoint As Selection
'        Get
'            Return _EndPoint
'        End Get
'    End Property

'    Private _Dir As SelectionDirection
'    Public ReadOnly Property Direction As SelectionDirection
'        Get
'            Return _Dir
'        End Get
'    End Property

'    Public Structure Selection

'        ''' <summary>
'        ''' Creates a new instance of the Selection object and fills its propertes
'        ''' </summary>
'        ''' <param name="CA">CA</param>
'        ''' <param name="SS">SS</param>
'        ''' <param name="SE">SE</param>
'        Public Sub New(ByVal CA As MathElement, ByVal SS As MathElement, ByVal SE As MathElement)
'            CommonAncestror = CA : SelectionStart = SS : SelectionEnd = SE
'        End Sub

'        Public CommonAncestror As MathElement
'        Public SelectionStart As MathElement
'        Public SelectionEnd As MathElement

'    End Structure

'    Public Enum SelectionDirection
'        LTR = 0
'        RTL = -1
'    End Enum

'    Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of MathElement) Implements System.Collections.Generic.IEnumerable(Of MathElement).GetEnumerator
'        Return GetSelectedElements().GetEnumerator()
'    End Function

'    Private Function GetGenericEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
'        Return GetEnumerator()
'    End Function

'    Private Sub CA_END_ChildAdded(ByVal sender As Object, ByVal e As MathElement.TreeEventArgs) Handles CA_END.ChildAdded
'        If EndPoint.SelectionEnd Is Nothing Then
'            SetSelection(New Selection(CA_END, CA_END.LastChild, Nothing), PointToChange:=SelectionPointType.EndPoint)
'        Else
'            SetSelection(New Selection(CA_END, CA_END.Children.Before(_EndPoint.SelectionEnd), _EndPoint.SelectionEnd), PointToChange:=SelectionPointType.EndPoint)
'        End If
'    End Sub

'    Private Sub CA_END_ChildRemoved(ByVal sender As Object, ByVal e As MathElement.TreeEventArgs) Handles CA_END.ChildRemoved

'        '
'        ' Verify that the newly removed child don't make the Selection EndPoint invalid!
'        '
'        If (_EndPoint.SelectionEnd Is Nothing) OrElse (CA_END.Children.Contains(_EndPoint.SelectionEnd)) Then
'            SetSelection(New Selection(CA_END, CA_END.Children.Before(_EndPoint.SelectionEnd), _EndPoint.SelectionEnd), PointToChange:=SelectionPointType.EndPoint)
'        Else
'            If (_EndPoint.SelectionStart Is Nothing) OrElse (CA_END.Children.Contains(_EndPoint.SelectionStart)) Then
'                SetSelection(New Selection(CA_END, _EndPoint.SelectionStart, CA_END.Children.After(_EndPoint.SelectionStart)), PointToChange:=SelectionPointType.EndPoint)
'            Else
'                SetSelection(New Selection(CA_END, CA_END.LastChild, Nothing), PointToChange:=SelectionPointType.EndPoint)
'            End If
'        End If

'    End Sub

'    Private Sub CA_START_ChildAdded(ByVal sender As Object, ByVal e As MathElement.TreeEventArgs) Handles CA_START.ChildAdded
'        If StartPoint.SelectionStart Is Nothing Then
'            SetSelection(New Selection(CA_START, Nothing, CA_START.FirstChild), PointToChange:=SelectionPointType.StartPoint)
'        Else
'            SetSelection(New Selection(CA_START, _StartPoint.SelectionStart, CA_START.Children.After(_StartPoint.SelectionStart)), PointToChange:=SelectionPointType.StartPoint)
'        End If
'    End Sub

'    Private Sub CA_START_ChildRemoved(ByVal sender As Object, ByVal e As MathElement.TreeEventArgs) Handles CA_START.ChildRemoved

'        '
'        ' Verify that the newly removed child don't make the Selection StartPoint invalid!
'        '
'        If (_StartPoint.SelectionStart Is Nothing) OrElse (CA_START.Children.Contains(_StartPoint.SelectionStart)) Then
'            SetSelection(New Selection(CA_START, _StartPoint.SelectionStart, CA_START.Children.After(_StartPoint.SelectionStart)), PointToChange:=SelectionPointType.StartPoint)
'        Else
'            If (_StartPoint.SelectionEnd Is Nothing) OrElse (CA_END.Children.Contains(_StartPoint.SelectionEnd)) Then
'                SetSelection(New Selection(CA_START, CA_START.Children.Before(_StartPoint.SelectionEnd), _StartPoint.SelectionEnd), PointToChange:=SelectionPointType.StartPoint)
'            Else
'                SetSelection(New Selection(CA_END, CA_END.LastChild, Nothing), PointToChange:=SelectionPointType.StartPoint)
'            End If
'        End If

'    End Sub
'End Class
