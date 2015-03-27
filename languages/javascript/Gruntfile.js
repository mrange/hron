/*jshint node: true, strict: false */

// -------------------------- grunt -------------------------- //

module.exports = function( grunt ) {

  grunt.initConfig({
    pkg: grunt.file.readJSON('package.json'),

    'string-replace': {
      inline: {
        files: { 'hron.js' : 'hron.js' },
        options: {
          replacements: [
            {
              pattern: /\* HRON v[\d\.]*/,
              replacement: "* HRON v<%= pkg.version %>"
            }
          ]
        }
      }
    },

    nodeunit: {
      all: ['test-nodeunit.js']
    }, 

    jshint: {
      src: [ 'hron.js' ],
      options: grunt.file.readJSON('.jshintrc')
    },

    uglify: {
      pkgd: {
        files: {
          'hron.min.js': [ 'hron.js' ]
        },
        options: {
          banner: '/*! <%= pkg.name %> version: <%= pkg.version %> */\n'
        }
      }
    }

  });

  grunt.loadNpmTasks('grunt-contrib-jshint');
  grunt.loadNpmTasks('grunt-contrib-uglify');
  grunt.loadNpmTasks('grunt-contrib-nodeunit');
  grunt.loadNpmTasks('grunt-string-replace');

  grunt.registerTask( 'default', [
    'string-replace',
    'nodeunit',
    'jshint',
    'uglify'
  ]);

};
