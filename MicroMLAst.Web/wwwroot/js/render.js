document.getElementById('btn').addEventListener('click', async () => {
  const code = document.getElementById('code').value;
  const res  = await fetch('/api/parse', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ code })
  });
  const { mermaid: graphDef } = await res.json();

  const container = document.getElementById('diagram');
  const graphId   = 'graph-' + Date.now();

  // NEW: await the promise-based render
  const { svg, bindFunctions } = await mermaid.render(graphId, graphDef);
  
  // inject the SVG and bind any event handlers
  container.innerHTML = svg;
  bindFunctions(container);
});
