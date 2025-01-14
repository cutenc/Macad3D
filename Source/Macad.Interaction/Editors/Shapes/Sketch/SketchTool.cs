﻿using Macad.Core.Shapes;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Macad.Interaction.Editors.Shapes
{
    public class SketchTool : WorkspaceControl
    {
        public bool IsActive { get; private set; }
        public SketchEditorTool SketchEditorTool { get; private set; }
        public Sketch Sketch { get; private set; }

        List<ToolAction> _Actions;

        //--------------------------------------------------------------------------------------------------

        public bool Start(SketchEditorTool sketchEditorTool)
        {
            SketchEditorTool = sketchEditorTool;
            Sketch = sketchEditorTool?.Sketch;
            WorkspaceController = sketchEditorTool?.WorkspaceController;
            if (WorkspaceController == null || Sketch == null)
                return false;

            bool started = OnStart();

            IsActive = started;
            return started;
        }

        //--------------------------------------------------------------------------------------------------

        public void Stop()
        {
            if (!IsActive)
                return;

            IsActive = false;
            SketchEditorTool.StopTool();

            OnStop();
            Cleanup();

            Debug.Assert(CleanedUp);
        }

        //--------------------------------------------------------------------------------------------------

        protected virtual bool OnStart()
        {
            return false;
        }

        //--------------------------------------------------------------------------------------------------

        protected virtual void OnStop() {}
        
        //--------------------------------------------------------------------------------------------------

        protected override void Cleanup()
        {
            _StopToolActions();
            base.Cleanup();
        }

        //--------------------------------------------------------------------------------------------------

        #region Actions

        protected bool StartAction(ToolAction toolAction)
        {
            if (SketchEditorTool.StartToolAction(toolAction))
            {
                _Actions ??= new();
                _Actions.Add(toolAction);
                return true;
            }
            return false;
        }

        //--------------------------------------------------------------------------------------------------

        protected void StopAction(ToolAction toolAction)
        {
            if (toolAction == null)
                return;

            _Actions?.Remove(toolAction);
            SketchEditorTool.StopToolAction(toolAction);
        }

        //--------------------------------------------------------------------------------------------------

        void _StopToolActions()
        {
            if (_Actions != null)
            {
                // Copy reference to disable chaning the enumeration
                var actions = _Actions;
                _Actions = null;
                foreach (var action in actions)
                {
                    StopAction(action);
                }
            }
        }

        //--------------------------------------------------------------------------------------------------

        #endregion
    }
}