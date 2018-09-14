using Ooui;
using TabNoc.MyOoui.HtmlElements;
using TabNoc.MyOoui.Interfaces.AbstractObjects;

namespace TabNoc.MyOoui.UiComponents
{
	public class Chart : StylableElement
	{
		public Chart(int width, int heigth) : base("div")
		{
			Canvas canvas = (Canvas)AppendChild(new Canvas()
			{
				Width = width,
				Height = heigth,
				Style = { Height = 400, Width = 400 }
			});

			//AppendChild(new Script().SetSource(""));

			AppendChild(new Script().SetContent($@"
$.getScript( ""https://cdnjs.cloudflare.com/ajax/libs/Chart.js/2.7.2/Chart.min.js"", function( data, textStatus, jqxhr ) {{
  console.log( data ); // Data returned
  console.log( textStatus ); // Success
  console.log( jqxhr.status ); // 200
  console.log( ""Load was performed."" );
	console.log('loading Chart...');
	var ctx = document.getElementById(""{canvas.Id}"").getContext('2d');
	var myChart = new Chart(ctx, {{
	    type: 'line',
	    data: {{
	        labels: [""Red"", ""Blue"", ""Yellow"", ""Green"", ""Purple"", ""Orange""],
	        datasets: [{{
	            label: '# of Votes',
	            data: [12, 19, 3, 5, 2, 3],
	            backgroundColor: [
	                'rgba(255, 99, 132, 0.2)',
	                'rgba(54, 162, 235, 0.2)',
	                'rgba(255, 206, 86, 0.2)',
	                'rgba(75, 192, 192, 0.2)',
	                'rgba(153, 102, 255, 0.2)',
	                'rgba(255, 159, 64, 0.2)'
	            ],
	            borderColor: [
	                'rgba(255,99,132,1)',
	                'rgba(54, 162, 235, 1)',
	                'rgba(255, 206, 86, 1)',
	                'rgba(75, 192, 192, 1)',
	                'rgba(153, 102, 255, 1)',
	                'rgba(255, 159, 64, 1)'
	            ],
	            borderWidth: 1
	        }}]
	    }},
	    options: {{
	        scales: {{
	            yAxes: [{{
	                ticks: {{
	                    beginAtZero:true
	                }}
	            }}]
	        }}
	    }}
	}});
	console.log('Chart loaded!');
}});
"));
		}
	}
}
