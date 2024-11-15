window.breakpointService = {
    breakpoints: {
        sm: 640,
        md: 768,
        lg: 1024,
        xl: 1280,
        '2xl': 1536
    },
    currentBreakpoint: '',
    initialize: function (dotNetHelper) {
        this.dotNetHelper = dotNetHelper;
        this.updateBreakpoint();
        window.addEventListener('resize', this.updateBreakpoint.bind(this));
    },
    updateBreakpoint: function () {
        const width = window.innerWidth;
        let newBreakpoint = '';

        if (width >= this.breakpoints['2xl']) {
            newBreakpoint = '2xl';
        } else if (width >= this.breakpoints.xl) {
            newBreakpoint = 'xl';
        } else if (width >= this.breakpoints.lg) {
            newBreakpoint = 'lg';
        } else if (width >= this.breakpoints.md) {
            newBreakpoint = 'md';
        } else if (width >= this.breakpoints.sm) {
            newBreakpoint = 'sm';
        } else {
            newBreakpoint = 'xs';
        }

        if (newBreakpoint !== this.currentBreakpoint) {
            this.currentBreakpoint = newBreakpoint;
            this.dotNetHelper.invokeMethodAsync('OnBreakpointChangedInClient', newBreakpoint);
        }
    },
    getCurrentBreakpoint: function () {
        return this.currentBreakpoint;
    }
};
