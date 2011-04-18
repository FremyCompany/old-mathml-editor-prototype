'?
'? Split MathElement into two classes: MathNode and MathElement (ME inherits from MN)
'?

Partial Public MustInherit Class MathElement

    '++ 
    '++ XML Hierarchy
    '++

    Private _Parent As MathElement
    Public Property Parent As MathElement
        Get
            Return _Parent
        End Get
        Set(ByVal value As MathElement)
            If _Parent IsNot Nothing Then
                ' REMOVING THE CURRENT PARENT
                If value Is Nothing Then
                    If Parent.Children.Contains(Me) Then
                        Throw New InvalidOperationException("Reseting the Parent property wasn't posssible because the parent still claims it owns the current element.")
                    Else

                        ' Perform some cleanup
                        _Parent = Nothing
                        _ParentDocument = Nothing
                        _Selection = Nothing

                        ' Raise the corresponding event
                        RaiseEvent RemovedFromParent(Me, EventArgs.Empty)

                    End If
                Else
                    ' TODO: Maybe we can check if value is _Parent before throwing
                    ' For now, we shall not do so, in order to find duplicate
                    ' code errors
                    Throw New InvalidOperationException("Unable to modify the parent of an element after it has been set. Use Clone() to get a parent-free copy of this element, or remove it from its current parent.")
                End If
            Else
                ' ADDING A NEW PARENT
                If value.Children.IsValidChild(Me) Then

                    ' Perform the change
                    _Parent = value

                    ' Raise the corresponding event
                    RaiseEvent AttachedToParent(Me, EventArgs.Empty)

                Else
                    Throw New ArgumentException("This element is not recognised as a valid child of its new parent.", "Parent")
                End If
            End If
        End Set
    End Property

    Public ReadOnly Property Root As MathElement
        Get
            If _ParentDocument IsNot Nothing Then Return _ParentDocument
            If _Parent Is Nothing Then Return Me
            Return Parent.Root
        End Get
    End Property

    Private _ParentDocument As MathDocument
    Public ReadOnly Property ParentDocument As MathDocument
        Get

            ' Cache the parent document, if not already done
            If _ParentDocument Is Nothing Then
                If Parent IsNot Nothing Then
                    _ParentDocument = Parent.ParentDocument
                ElseIf TryCast(Me, MathDocument) IsNot Nothing Then
                    _ParentDocument = DirectCast(Me, MathDocument)
                End If
            End If

            ' Return the cached parent document
            Return _ParentDocument

        End Get
    End Property

    Public ReadOnly Property NextSibling As MathElement
        Get
            If _Parent Is Nothing Then Return Nothing
            Return _Parent.Children.After(Me)
        End Get
    End Property

    Public ReadOnly Property PreviousSibling As MathElement
        Get
            If _Parent Is Nothing Then Return Nothing
            Return _Parent.Children.Before(Me)
        End Get
    End Property

    Public Function IsBefore(ByVal Elm As MathElement) As Boolean

        If Elm Is Nothing Then Return True
        If Elm.Parent IsNot Parent Then Return False

        Do 'PreviousSiblig
            If (Elm Is Me) Then Return True
            Elm = Parent.Children.Before(Elm)
        Loop While (Elm IsNot Nothing)

        Return False
    End Function

    Public Function IsAfter(ByVal Elm As MathElement) As Boolean

        If Elm Is Nothing Then Return True
        Return Elm.IsBefore(Me)

    End Function

    Public Function Clone() As MathElement
        Clone = Me.Clone_Internal()
        If _IsFontStretchDefined Then Clone.FontStretch = Me.FontStretch
        If _IsFontWeightDefined Then Clone.FontWeight = Me.FontWeight
        If _IsFontStyleDefined Then Clone.FontStyle = Me.FontStyle
        If _IsFontFamilyDefined Then Clone.FontFamily = Me.FontFamily
        If _IsFontSizeDefined Then Clone.FontSize = Me.FontSize
        If _IsForegroundDefined Then Clone.Foreground = Me.Foreground
        If _IsBackgroundDefined Then Clone.Background = Me.Background
        Return Clone
    End Function
    Public MustOverride Function Clone_Internal() As MathElement

    '++
    '++ XML Children
    '++

    Protected _Children As ChildrenHelper
    Public ReadOnly Property Children As ChildrenHelper
        Get
            If _Children Is Nothing Then _Children = Nothing
            Return _Children
        End Get
    End Property

    Public Sub AddChild(ByVal NewChild As MathElement)
        Children.Add(NewChild)
    End Sub

    Public Sub RemoveChild(ByVal OldChild As MathElement)
        Children.Remove(OldChild)
    End Sub

    Public Sub ReplaceChild(ByVal OldChild As MathElement, ByVal NewChild As MathElement)
        Children.Replace(OldChild, NewChild)
    End Sub

    Public ReadOnly Property FirstChild As MathElement
        Get
            Return Children.First
        End Get
    End Property

    Public ReadOnly Property LastChild As MathElement
        Get
            Return Children.Last
        End Get
    End Property

    Public ReadOnly Property TreeDepht As Integer
        Get
            TreeDepht = 0 : Dim P As MathElement = Me
            While P.Parent IsNot Nothing
                TreeDepht += 1 : P = P.Parent
            End While
            Return TreeDepht
        End Get
    End Property

    Public Function GetCommonAncestrorWith(ByVal el1 As MathElement) As MathElement
        Return GetCommonAncestrorBetween(el1, Me)
    End Function

    Public Shared Function GetCommonAncestrorBetween(ByVal el1 As MathElement, ByVal el2 As MathElement) As MathElement

        Dim Delta As Integer = el2.TreeDepht - el1.TreeDepht
        If Delta > 0 Then
            For x = 1 To Delta
                el2 = el2.Parent
            Next
        ElseIf Delta < 0 Then
            For x = 1 To -Delta
                el1 = el1.Parent
            Next
        End If

        While el2 IsNot el1
            el1 = el1.Parent : el2 = el2.Parent
        End While

        Return el1

    End Function

    '++
    '++ XML Events
    '++

    ''' <summary>
    ''' Risen when a property of the current element changed, or a property of one of its children
    ''' </summary>
    ''' <remarks>
    ''' When this event is risen, the layout of the current element should be recomputed.
    ''' </remarks>
    Public Event Changed As EventHandler
    Public Sub RaiseChanged()
        If ShouldRaiseChanged = 0 Then
            RaiseEvent Changed(Me, EventArgs.Empty)
        Else
            ChangePendings += 1
        End If
    End Sub

    ' TODO: Use StartBatchProcess in the current code
    Private ShouldRaiseChanged, ChangePendings As Integer
    Public Sub StartBatchProcess()
        ShouldRaiseChanged += 1
    End Sub

    Public Sub StopBatchProcess()
        ShouldRaiseChanged = 0
        If ChangePendings <> 0 Then
            ChangePendings = 0
            RaiseChanged()
        End If
    End Sub

#If DEBUG Then
    Public LastChangeTimestamp As Date
#End If

    Private Sub MathElement_Changed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Changed
#If DEBUG Then
        LastChangeTimestamp = Date.Now
#End If
        If _Parent IsNot Nothing Then Parent.RaiseChanged()
    End Sub

    ' TODO: RemovedFromParent
    Public Event AttachedToParent As EventHandler
    Public Event RemovedFromParent As EventHandler

    Public Event ChildAdded As EventHandler(Of TreeEventArgs)
    Public Event ChildRemoved As EventHandler(Of TreeEventArgs)

    Public Class TreeEventArgs : Inherits EventArgs

        Public Property Argument As MathElement
        Public Sub New(ByVal Argument As MathElement)
            Me.Argument = Argument
        End Sub

    End Class

End Class
